using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectWebFinal.Models;
using System.Security.Claims;
using System.Text;

namespace ProjectWebFinal.Controllers {
    public class AccountController : Controller {

        ManagerController context = new ManagerController();
        HttpClientHandler _clientHandler = new HttpClientHandler();

        public AccountController() {
            _clientHandler.ServerCertificateCustomValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => { return true; };
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Register() {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Manager manager) {

            var data = await context.GetManagers();
            bool emailExisting = data.Any(e => e.Email == manager.Email);
            if (!emailExisting) {
                try {
                    Manager? mng = new Manager();
                    using (var httpClient = new HttpClient(_clientHandler)) {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(manager), Encoding.UTF8, "application/json");
                        using (var response = await httpClient.PostAsync("https://localhost:7085/api/Manager", content)) {
                            string strJson = await response.Content.ReadAsStringAsync();
                            mng = JsonConvert.DeserializeObject<Manager>(strJson);
                            if (ModelState.IsValid) {
                                return RedirectToAction("Login", "Account");
                            }
                        }
                    }
                    return View(mng);
                } catch {
                    return View();
                }
            } else {
                TempData["emailExist"] = "บัญชีนี้เป็นสมาชิกแล้ว";
                return View();
            }
        }

        public IActionResult Login() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login model) {

            var data = await context.GetManagers();

            if (ModelState.IsValid) {
                Manager? manager = data.Where(m => m.Email == model.Email).SingleOrDefault();
                
                if (manager != null) {
                    bool isValid = (manager.Email == model.Email && BCrypt.Net.BCrypt.Verify(model.Password, manager.Password));
                    if (isValid) {
                        var identity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, manager.FirstName + " " + manager.LastName) },
                            CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        HttpContext?.Session.SetString("Name", model.Email);
                        return RedirectToAction("Index", "Manager");
                    } else {
                        TempData["errorPass"] = "รหัสผ่านไม่ถูกต้อง";
                        return View();
                    }
                } else {
                    TempData["errorEmail"] = "ไม่พบผู้ใช้";
                    return View();
                }
            } else {
                return View();
            }
        }

        public IActionResult Logout() {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var storedCookies = Request.Cookies.Keys;
            foreach (var cookie in storedCookies) { 
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Login", "Account");
        }

    }
}
