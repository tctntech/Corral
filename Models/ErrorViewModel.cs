namespace Corral.Models
{
    public class ErrorViewModel
    {
        
        
            public string? RequestId { get; set; }
            public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

            // New properties for more detailed error information
            public string? ErrorMessage { get; set; }
            public string? StackTrace { get; set; }
            public string? ExceptionDetails { get; set; }
        }
    }



