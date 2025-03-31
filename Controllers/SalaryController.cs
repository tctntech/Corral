using Corral.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;  // Added for async task support

namespace Corral.Controllers
{
    public class SalaryController : Controller  // Changed to MVC controller
    {
        private readonly string _connectionString;
        private readonly ILogger<SalaryController> _logger;

        // Injecting ILogger<SalaryController> into the constructor for logging
        public SalaryController(IConfiguration configuration, ILogger<SalaryController> logger)
        {
            _connectionString = configuration.GetConnectionString("TexasCorralDB");
            _logger = logger; // Assign the logger to the private field
        }

        // Action method to display the salary discrepancies
        public async Task<IActionResult> Index(DateTime bdate)
        {
            var discrepancies = await GetSalaryDiscrepanciesAsync(bdate);
            return View(discrepancies);  // Pass discrepancies to the view
        }

        // Method to call stored procedure and get the data
        private async Task<IEnumerable<SalaryDiscrepancy>> GetSalaryDiscrepanciesAsync(DateTime bdate)
        {
            var salaryDiscrepancies = new List<SalaryDiscrepancy>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("dbo.TCsp_SalaryActualVPaidAllIssues", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@bdate", SqlDbType.DateTime) { Value = bdate });

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        salaryDiscrepancies.Add(new SalaryDiscrepancy
                        {
                            Storeid = reader.GetInt32(reader.GetOrdinal("Storeid")),
                            Staffuid = reader.GetInt32(reader.GetOrdinal("Staffuid")),
                            Lastname = reader.GetString(reader.GetOrdinal("Lastname")),
                            Firstname = reader.GetString(reader.GetOrdinal("Firstname")),
                            Payrollid = reader.GetString(reader.GetOrdinal("Payrollid")),
                            ActualWorked = reader.GetDecimal(reader.GetOrdinal("ActualWorked")),
                            ActPaidHours = reader.GetDecimal(reader.GetOrdinal("ActPaidHours")),
                            Difference = reader.GetDecimal(reader.GetOrdinal("Difference"))
                        });
                    }
                }
            }

            return salaryDiscrepancies;
        }
    }
}
