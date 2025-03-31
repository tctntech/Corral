using Corral.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Corral.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            // Get the current exception details, if any.
            var exception = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

            // Create a model to pass error information
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,

                // Pass the exception details to the model for display
                ErrorMessage = exception?.Message,
                StackTrace = exception?.StackTrace,
                ExceptionDetails = exception?.ToString()
            };

            return View(model); // Return the error view with the populated model
        }
    }
}
