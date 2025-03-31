using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
namespace Corral.Controllers
{
    public class TimeClockViewController : Controller
    {
        private readonly ILogger<TimeClockViewController> _logger;

        public TimeClockViewController(ILogger<TimeClockViewController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/TimeClock/AdminFeatures")]
        public IActionResult AdminFeatures()
        {
            _logger.LogInformation("AdminFeatures page requested.");
            return View("AdminFeatures"); // This should match the view file name
        }

        [HttpGet("/TimeClock/Index")]
        public IActionResult Index()
        {
            _logger.LogInformation("TimeClock index page requested.");
            return View("Index");
        }
    }
}
