using Microsoft.AspNetCore.Mvc;
using SimuladorCUM.Models;
using SimuladorCUM.ViewModels;
using System.Text;
using System.Text.Json;

namespace SimuladorCUM.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            // Aquí después cargarás datos con el token
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            var loginUrl = $"{_configuration["UniversityApi:LoginUrl"]}";

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(loginUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Credenciales inválidas o error en el servidor.");
                return View(model);
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson);

            if (loginResponse?.Status != 200 || string.IsNullOrWhiteSpace(loginResponse.Access_Token))
            {
                ModelState.AddModelError("", "No se pudo iniciar sesión.");
                return View(model);
            }

            // Guardar en sesión
            HttpContext.Session.SetString("access_token", loginResponse.Access_Token);
            HttpContext.Session.SetString("program", loginResponse.Program);
            HttpContext.Session.SetString("refresh_token", loginResponse.Refresh_Token);

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
