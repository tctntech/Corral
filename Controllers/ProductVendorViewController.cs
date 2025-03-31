using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Corral.Models;
using Newtonsoft.Json;

namespace Corral.Controllers
{
    public class ProductVendorViewController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor to inject IHttpClientFactory
        public ProductVendorViewController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Method to get the list of products
        public async Task<IActionResult> Products()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetStringAsync("http://localhost:5050/api/ProductVendor/GetAll");
                var products = JsonConvert.DeserializeObject<List<ProductAbsolute>>(response);
                return View(products);
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions (e.g., network errors, API down)
                ViewBag.ErrorMessage = "Unable to fetch data from the server. Please try again later.";
                return View("Error"); // Return an error view
            }
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        // Method to get product details by ID
        public async Task<JsonResult> GetProductDetails(int id)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetStringAsync($"http://localhost:5050/api/ProductVendor/GetById/{id}");
                var product = JsonConvert.DeserializeObject<ProductAbsolute>(response);
                return Json(product);
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions (e.g., network errors, API down)
                return Json(new { error = "Unable to fetch product details" });
            }
        }
    }
}
