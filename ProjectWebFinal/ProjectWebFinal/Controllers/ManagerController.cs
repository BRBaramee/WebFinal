using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectWebFinal.Models;
using System.Text;

namespace ProjectWebFinal.Controllers {
    [Authorize]
    public class ManagerController : Controller {
        HttpClientHandler _clientHandler = new HttpClientHandler();

        public ManagerController() {
            _clientHandler.ServerCertificateCustomValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => { return true; };
        }

        // GET: CallManagerController
        public async Task<ActionResult> Index(String SearchText) {
            List<Manager>? managers = await GetManagers();
            if (!string.IsNullOrEmpty(SearchText))
            {

                managers = managers!.Where(x => x.FirstName!.ToLower().Contains(SearchText.ToLower()) || x.LastName!.ToLower().Contains(SearchText.ToLower())).ToList();

            }


            return View(managers);
        }

  

        [HttpGet]
        public async Task<List<Manager>?> GetManagers() {
            List<Manager>? managers = new List<Manager>();
            using (var httpClient = new HttpClient(_clientHandler)) {
                using (var response = await httpClient.GetAsync("https://localhost:7085/api/Manager")) {
                    string strJson = await response.Content.ReadAsStringAsync();
                    managers = JsonConvert.DeserializeObject<List<Manager>>(strJson);
                }
            }
            return managers;
        }

        // GET: ManagerController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Manager? manager = new Manager();
            using (var httpClient = new HttpClient(_clientHandler))
            {
                using (var response = await httpClient.GetAsync("https://localhost:7085/api/Manager/id?id="+id))
                {
                    string strJson = await response.Content.ReadAsStringAsync();
                    manager = JsonConvert.DeserializeObject<Manager>(strJson);
                }
            }
            return View(manager);
        }

        // GET: ManagerController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: ManagerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Manager manager) {
            try {
                Manager? mng = new Manager();
                using (var httpClient = new HttpClient(_clientHandler))
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(manager), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync("https://localhost:7085/api/Manager", content))
                    {
                        string strJson = await response.Content.ReadAsStringAsync();
                        mng = JsonConvert.DeserializeObject<Manager>(strJson);
                        if (ModelState.IsValid)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                return View(mng);
            }
            catch
            {
                return View();
            }
        }

        // GET: ManagerController/Edit/5
        public async Task<ActionResult> Edit(int id) {
            Manager? manager = new Manager();
            using (var httpClient = new HttpClient(_clientHandler))
            {
                using (var response = await httpClient.GetAsync("https://localhost:7085/api/Manager/id?id=" + id))
                {
                    string strJson = await response.Content.ReadAsStringAsync();
                    manager = JsonConvert.DeserializeObject<Manager>(strJson);
                }
            }
            return View(manager);
        }

        // POST: ManagerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id,Manager manager) {
            Manager mng = new Manager();
            using (var httpClient = new HttpClient(_clientHandler))
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(manager), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("https://localhost:7085/api/Manager/" + id,content))
                {
                    
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ManagerController/Delete/5
        public async Task<ActionResult> Delete(int id) {
            string del = "";
            using (var httpClient = new HttpClient(_clientHandler))
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:7085/api/Manager/" + id))
                {
                    del = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: ManagerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }



    }
}
