using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Corral.Models;
using Newtonsoft.Json;


namespace Corral.Controllers
{
    public class VendorViewController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor to inject IHttpClientFactory
        public VendorViewController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Method to fetch the list of vendors
        public async Task<IActionResult> Vendors()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetStringAsync("http://localhost:5050/api/Vendor/GetAll");
                var vendors = JsonConvert.DeserializeObject<List<Vendor>>(response);
                return View(vendors);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Unable to fetch vendor data. Please try again later.";
                return View("Error");
            }
        }

        // Dashboard view
        public IActionResult Dashboard()
        {
            return View();
        }

        // Method to get vendor details by ID
       
        public async Task<JsonResult> GetVendorDetails(int id)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetStringAsync($"http://localhost:5050/api/Vendor/{id}");
                var vendor = JsonConvert.DeserializeObject<Vendor>(response);
                return Json(vendor);
            }
            catch (HttpRequestException)
            {
                return Json(new { error = "Unable to fetch vendor details" });
            }
        }

    }
}
