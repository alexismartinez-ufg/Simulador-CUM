using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimuladorCUM.Models;
using SimuladorCUM.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace SimuladorCUM.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("access_token");
            var carnet = HttpContext.Session.GetString("carnet");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(carnet))
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_configuration["UniversityApi:PlanEstudioUrl"]}/{carnet}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error al obtener los datos del plan de estudios.");
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(json);

            return View(apiResponse);
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

            try
            {
                var client = _httpClientFactory.CreateClient();
                var loginUrl = $"{_configuration["UniversityApi:LoginUrl"]}";

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(loginUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var showDetail = _env.IsDevelopment() || _configuration.GetValue<bool>("AppSettings:ShowDetailedErrors");

                    string detail = showDetail
                        ? await response.Content.ReadAsStringAsync()
                        : "Credenciales inválidas o error en el servidor.";

                    ModelState.AddModelError("", detail);
                    return View(model);
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseJson);

                if (loginResponse?.Status != 200 || string.IsNullOrWhiteSpace(loginResponse.Access_Token))
                {
                    ModelState.AddModelError("", "No se pudo iniciar sesión. " + loginResponse?.Message);
                    return View(model);
                }

                // Extraer el carnet del mensaje: "Bienvenido/a MB101421"
                var carnetMatch = Regex.Match(loginResponse.Message ?? "", @"\b[A-Z]{2}\d{6}\b");
                var carnet = carnetMatch.Success ? carnetMatch.Value : string.Empty;

                // Guardar en sesión
                HttpContext.Session.SetString("carnet", carnet);
                HttpContext.Session.SetString("access_token", loginResponse.Access_Token);
                HttpContext.Session.SetString("program", loginResponse.Program);
                HttpContext.Session.SetString("refresh_token", loginResponse.Refresh_Token);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var showDetail = _env.IsDevelopment() || _configuration.GetValue<bool>("AppSettings:ShowDetailedErrors");

                string message = showDetail ? ex.ToString() : "Ocurrió un error inesperado. Intente nuevamente más tarde.";
                ModelState.AddModelError("", message);
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
