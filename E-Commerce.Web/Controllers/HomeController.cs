using E_Commerce.Domain.Models;
using E_Commerce.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace E_Commerce.Web.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IWebHostEnvironment env) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IWebHostEnvironment _env = env;
        public IActionResult Index()
        {
            var filePath = Path.Combine(
                  _env.ContentRootPath,
                  "data",
                  "products.json");

            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogWarning("Products file not found at {Path}", filePath);
                return View(new List<FeaturedCategory>());
            }

            var json = System.IO.File.ReadAllText(filePath);

            var products = JsonSerializer.Deserialize<List<FeaturedCategory>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(products ?? []);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
