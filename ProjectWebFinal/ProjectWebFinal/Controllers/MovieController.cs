using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectWebApiFinal.Models;
using ProjectWebFinal.Models;
using System.Text;


namespace ProjectWebFinal.Controllers {
    [Authorize]
    public class MovieController : Controller {
        HttpClientHandler _clientHandler = new HttpClientHandler();

        

        public MovieController() {
            _clientHandler.ServerCertificateCustomValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => { return true; };
        }

        // GET: MovieController
        public async Task<ActionResult> Index(String SearchText) {
            List<Movie>? movies = await GetMovies();
            if (!string.IsNullOrEmpty(SearchText))
            {
                movies = movies!.Where(x => x.Title!.ToLower().Contains(SearchText.ToLower()) || x.Year!.ToLower().Contains(SearchText.ToLower())).ToList();
            }
            return View(movies);
        }



        [HttpGet]
        public async Task<List<Movie>?> GetMovies() {
            List<Movie>? movies = new List<Movie>();
            using (var httpClient = new HttpClient(_clientHandler)) {
                using (var response = await httpClient.GetAsync("https://localhost:7085/api/Movie")) {
                    string strJson = await response.Content.ReadAsStringAsync();
                    movies = JsonConvert.DeserializeObject<List<Movie>>(strJson);
                }
            }
            return movies;
        }

        // GET: MovieController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            {
                Movie? movie = new Movie();
                using (var httpClient = new HttpClient(_clientHandler))
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7085/api/Movie/id?id=" + id))
                    {
                        string strJson = await response.Content.ReadAsStringAsync();
                        movie = JsonConvert.DeserializeObject<Movie>(strJson);
                    }
                }
                return View(movie);
            }
        }

        // GET: MovieController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: MovieController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Movie model, IFormFile formFile) {
            string? path = UploadImage(formFile);



            try {
                if (path == null) {
                    throw new Exception();
                } else { 
                    Movie newMovie = new Movie() {
                        Title = model.Title,
                        Year = model.Year,
                        Poster = path
                    };
                Movie? movie = new Movie();
                using (var httpClient = new HttpClient(_clientHandler)) {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(newMovie), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync("https://localhost:7085/api/Movie", content)) {
                        string strJson = await response.Content.ReadAsStringAsync();
                        movie = JsonConvert.DeserializeObject<Movie>(strJson);
                        if (ModelState.IsValid) {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                TempData["imgError"] = "โปรดเลือกรูปภาพ";
                return View(movie);
                }
            } catch {
                TempData["imgError"] = "โปรดเลือกรูปภาพ";
                return View(model);
            }
        }

        [HttpPost]
        public string? UploadImage(IFormFile file) {
            Random rnd = new Random();
            try {
                if (file != null) {
                    int r = rnd.Next();
                    string fileName = file.FileName;
                    fileName = Path.GetFileName(fileName);
                    string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", r+fileName);
                    var stream = new FileStream(uploadpath, FileMode.Create);
                    file.CopyToAsync(stream);
                    return "~/images/"+r+fileName;
                } else {
                    return null;
                }
            } catch {
                return null;
            }
        }

        // GET: MovieController/Edit/5
        public async Task<ActionResult> Edit(int id) {
            Movie? movie = new Movie();
            using (var httpClient = new HttpClient(_clientHandler))
            {
                using (var response = await httpClient.GetAsync("https://localhost:7085/api/Movie/id?id=" + id))
                {
                    string strJson = await response.Content.ReadAsStringAsync();
                    movie = JsonConvert.DeserializeObject<Movie>(strJson);
                }
            }
            return View(movie);
        }
        
        // POST: MovieController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Movie movie, IFormFile file)
        {
            
            string? path = UploadImage(file);

            if (path == null) {
                List<Movie> mm = await GetMovies();
                var m = mm.First(x => x.Id == id);
                movie.Poster = m.Poster;
            } else { 
                movie.Poster = path;
            }

            try {
                Movie mv = new Movie();
                using (var httpClient = new HttpClient(_clientHandler))
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(movie), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PutAsync("https://localhost:7085/api/Movie/" + id, content))
                    {
                        string strJson = await response.Content.ReadAsStringAsync();
                        mv = JsonConvert.DeserializeObject<Movie>(strJson);
                        if (ModelState.IsValid) {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            return RedirectToAction(nameof(Index));
            } catch {
                TempData["imgError"] = "โปรดเลือกรูปภาพ";
                return View(movie);
            }
        }

        // GET: MovieController/Delete/5
        public async Task<ActionResult> Delete(int id) {
            string del = "";
            using (var httpClient = new HttpClient(_clientHandler))
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:7085/api/Movie/" + id))
                {
                    del = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: MovieController/Delete/5
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
