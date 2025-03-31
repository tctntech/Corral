using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using Corral.Models;

namespace Corral.Controllers
{
    [ApiController]
    [Route("api/managers")]
    public class ManagerController : ControllerBase
    {
        private readonly string _connectionString;

        public ManagerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TexasCorralDB");
        }

        // ✅ GET: Fetch all managers
        [HttpGet]
        public IActionResult GetManagers()
        {
            var managers = new List<ManagerUser>();
            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new OdbcCommand("SELECT ManagerID, FullName, Username, Active, HomeStore, AccountActiveDate FROM [Manager].[tblManagers]", connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            managers.Add(new ManagerUser
                            {
                                ManagerID = reader.GetInt32(0),
                                FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1), // ✅ Only for retrieval
                                Username = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Active = reader.IsDBNull(3) ? "0" : reader.GetString(3).Trim(),
                                HomeStore = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                AccountActiveDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
                            });
                        }
                    }
                }
                return Ok(managers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ GET: Fetch manager by ID
        [HttpGet("{id}")]
        public IActionResult GetManagerById(int id)
        {
            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new OdbcCommand("SELECT ManagerID, FullName, Username, Active, HomeStore, AccountActiveDate FROM [Manager].[tblManagers] WHERE ManagerID = ?", connection))
                    {
                        command.Parameters.Add(new OdbcParameter("?", id));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var manager = new ManagerUser
                                {
                                    ManagerID = reader.GetInt32(0),
                                    FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1), // ✅ Only for retrieval
                                    Username = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                    Active = reader.IsDBNull(3) ? "0" : reader.GetString(3).Trim(),
                                    HomeStore = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                    AccountActiveDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
                                };
                                return Ok(manager);
                            }
                        }
                    }
                }
                return NotFound("Manager not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ POST: Create new manager (❌ Removed FullName)
        [HttpPost]
        public IActionResult CreateManager([FromBody] ManagerUser manager)
        {
            if (manager == null)
                return BadRequest("Invalid manager data.");

            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new OdbcCommand(@"INSERT INTO [Manager].[tblManagers] (Username, Active, HomeStore, AccountActiveDate) 
                                                  VALUES (?, ?, ?, ?)", connection)) // ❌ Removed FullName
                    {
                        command.Parameters.Add(new OdbcParameter("?", manager.Username));
                        command.Parameters.Add(new OdbcParameter("?", manager.Active));
                        command.Parameters.Add(new OdbcParameter("?", manager.HomeStore ?? (object)DBNull.Value));
                        command.Parameters.Add(new OdbcParameter("?", manager.AccountActiveDate ?? (object)DBNull.Value));

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            return Ok("Manager added successfully.");
                        else
                            return StatusCode(500, "Failed to add manager.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ PUT: Update manager (❌ Removed FullName)
        [HttpPut("{id}")]
        public IActionResult UpdateManager(int id, [FromBody] ManagerUser manager)
        {
            if (manager == null || id != manager.ManagerID)
                return BadRequest("Invalid manager data.");

            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new OdbcCommand(@"UPDATE [Manager].[tblManagers] 
                                                  SET Username = ?, Active = ?, HomeStore = ?, AccountActiveDate = ?
                                                  WHERE ManagerID = ?", connection)) // ❌ Removed FullName
                    {
                        command.Parameters.Add(new OdbcParameter("?", manager.Username));
                        command.Parameters.Add(new OdbcParameter("?", manager.Active));
                        command.Parameters.Add(new OdbcParameter("?", manager.HomeStore ?? (object)DBNull.Value));
                        command.Parameters.Add(new OdbcParameter("?", manager.AccountActiveDate ?? (object)DBNull.Value));
                        command.Parameters.Add(new OdbcParameter("?", id));

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            return Ok("Manager updated successfully.");
                        else
                            return NotFound("Manager not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ DELETE: Delete manager
        [HttpDelete("{id}")]
        public IActionResult DeleteManager(int id)
        {
            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new OdbcCommand("DELETE FROM [Manager].[tblManagers] WHERE ManagerID = ?", connection))
                    {
                        command.Parameters.Add(new OdbcParameter("?", id));

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            return Ok("Manager deleted successfully.");
                        else
                            return NotFound("Manager not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
