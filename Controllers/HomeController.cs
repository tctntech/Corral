using Corral.Models;
using Microsoft.AspNetCore.Mvc;

namespace Corral.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new HomeModel
            {
                Title = "Welcome to Texas Corral!" // Example dynamic data
            };
            return View(model);
        }
    }
}
