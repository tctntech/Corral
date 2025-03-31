using Corral.Models;
using System.Data.Odbc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Corral.Controllers
{
    public class AllLocationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AllLocationController> _logger;

        public AllLocationController(IConfiguration configuration, ILogger<AllLocationController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<StoreInfo> storeList = new List<StoreInfo>();

            try
            {
                var connectionString = _configuration.GetConnectionString("TexasCorralDB");  // Using appsettings.json
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT StoreNumber, StoreName, City, State 
                        FROM [TexasCorralQA].[General].[StoreInformation] 
                        WHERE Active = 1";  // Fetch only active stores

                    using (var command = new OdbcCommand(query, connection))
                    {
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            storeList.Add(new StoreInfo
                            {
                                StoreNumber = reader.GetInt32(0),
                                StoreName = reader.GetString(1),
                                Location = reader.GetString(2) + ", " + reader.GetString(3)  // City, State
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving store locations: " + ex.Message);
                ViewBag.ErrorMessage = "Failed to load store locations.";
            }

            return View(storeList);
        }
    }
}
