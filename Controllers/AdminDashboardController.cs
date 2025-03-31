using Microsoft.AspNetCore.Mvc;

namespace Corral.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            // Check if the user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Redirect to login page if not logged in
                return RedirectToAction("Login", "Account");
            }

            // Render the dashboard if logged in
            return View();
        }

        public IActionResult AdminReports()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public IActionResult TimeClock()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Explicitly return the TimeClock view
            return View("~/Views/TimeClock/Index.cshtml");
        }
    }
}
