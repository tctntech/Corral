using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.Odbc;
using System.Security.Cryptography;
using System.Text;

namespace Corral.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IConfiguration configuration, ILogger<AccountController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Render the login view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            try
            {
                _logger.LogInformation("Login method triggered");

                // Get connection string from appsettings.json
                var connectionString = _configuration.GetConnectionString("TexasCorralDB");
                _logger.LogInformation("Connection string retrieved");

                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Database connection opened successfully");

                    // SQL query to validate user
                    string query = @"
    SELECT ManagerID, FirstName, LastName, Password
    FROM [Manager].[tblManagers]
    WHERE Username = ? AND Active = 1";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        // Use positional parameters
                        command.Parameters.AddWithValue("?", username);

                        var reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            string storedPassword = reader["Password"].ToString();
                            _logger.LogInformation("Password fetched from database");

                            if (storedPassword == password)
                            {
                                // Successful login
                                _logger.LogInformation("Login successful");
                                HttpContext.Session.SetString("Username", username);
                                return RedirectToAction("Index", "AdminDashboard");
                            }
                            else
                            {
                                _logger.LogWarning("Invalid password");
                                ViewBag.ErrorMessage = "Invalid Username or Password.";
                                return View();
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Invalid username");
                            ViewBag.ErrorMessage = "Invalid Username or Password.";
                            return View();
                        }
                    }
               
                    }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                return View();
            }
        }

        

        // Method to verify password (use hashing and salt for security)
        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Example using SHA256 (you should consider using a stronger hashing algorithm like bcrypt or PBKDF2)
            using (var sha256 = SHA256.Create())
            {
                var enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
                var hashedPasswordBytes = sha256.ComputeHash(enteredPasswordBytes);
                var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);
                return hashedPassword == storedPasswordHash;
            }
        }
    }
}
