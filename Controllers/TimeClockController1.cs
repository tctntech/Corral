using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Corral.Models;
using System.IO;
using iText.Kernel.Pdf;  // For iText 7 PDF handling
using iText.Layout;       // For iText 7 Layout (Paragraph, Document, etc.)
using iText.Layout.Element;  // For iText 7 Layout Elements (like Paragraph)
using Microsoft.AspNetCore.Mvc;
using System.Data.Odbc;
using System.Globalization;
using System.Threading.Tasks;
using iText.Kernel.Font;
using iText.Layout.Properties;

namespace Corral.Controllers
{
    [Route("api/[controller]")]
    public class TimeClockController : Controller
    {
        private readonly string _connectionString =
            "Driver={ODBC Driver 17 for SQL Server};Server=192.168.5.15;Database=TexasCorralQA;Uid=zpatel;Pwd=@TCzp2219!;Trusted_Connection=no;";

        private readonly ILogger<TimeClockController> _logger;

        public TimeClockController(ILogger<TimeClockController> logger)
        {
            _logger = logger;
        }

        [HttpGet("AdminFeatures")]
        public IActionResult AdminFeatures() => View("AdminFeatures");


        [HttpGet("Index")]
        public IActionResult Index() => View("Index");


        [HttpGet("employees")]
        public IActionResult GetEmployees()
        {
            try
            {
                var employees = new List<object>();
                using (var connection = new OdbcConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ManagerID, FirstName + ' ' + LastName AS EmployeeName FROM [Manager].[tblManagers]";
                    using (var command = new OdbcCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new
                            {
                                EmployeeID = reader["ManagerID"],
                                EmployeeName = reader["EmployeeName"].ToString().Trim()
                            });
                        }
                    }
                }
                return Json(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching employees: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("entries")]
        public async Task<IActionResult> GetEntries([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? employeeId)
        {
            var timeClockEntries = new List<TimeClockEntry>();
            try
            {
                using (var conn = new OdbcConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    StringBuilder query = new StringBuilder(@"SELECT TM.TMID, TM.ManagerID, 
                        COALESCE(M.FirstName, '') + ' ' + COALESCE(M.LastName, '') AS EmployeeName, 
                        TM.StoreID, TM.Rate, TM.ServerTimeIn, TM.ServerTimeOut, TM.Approved, TM.ApprovedBy, 
                        COALESCE(A.FirstName, '') + ' ' + COALESCE(A.LastName, '') AS ApprovedByName 
                        FROM [TimeClock].[TimeManagement] TM 
                        LEFT JOIN [Manager].[tblManagers] M ON TM.ManagerID = M.ManagerID 
                        LEFT JOIN [Manager].[tblManagers] A ON TM.ApprovedBy = A.ManagerID WHERE 1=1");

                    if (startDate.HasValue) query.Append(" AND TM.ServerTimeIn >= ?");
                    if (endDate.HasValue) query.Append(" AND TM.ServerTimeIn <= ?");
                    if (employeeId.HasValue) query.Append(" AND TM.ManagerID = ?");

                    using (var cmd = new OdbcCommand(query.ToString(), conn))
                    {
                        if (startDate.HasValue) cmd.Parameters.AddWithValue("?", startDate.Value);
                        if (endDate.HasValue) cmd.Parameters.AddWithValue("?", endDate.Value);
                        if (employeeId.HasValue) cmd.Parameters.AddWithValue("?", employeeId.Value);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                timeClockEntries.Add(new TimeClockEntry
                                {
                                    TMID = reader.GetInt32(0),
                                    ManagerID = reader.GetInt32(1),
                                    EmployeeName = reader.GetString(2),
                                    StoreID = reader.GetInt32(3),
                                    Rate = reader.GetDecimal(4),
                                    ServerTimeIn = reader.GetDateTime(5),
                                    ServerTimeOut = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                    Approved = reader.GetBoolean(7),
                                    ApprovedBy = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                                    ApprovedByName = reader.GetString(9)
                                });
                            }
                        }
                    }
                }
                return Json(new { Data = timeClockEntries, TotalRecords = timeClockEntries.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching entries: {ex.Message}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpPost("clockin")]
        public async Task<IActionResult> ClockIn([FromBody] TimePunchRequest request)
        {
            _logger.LogInformation($"Clock-In Request Received: ManagerID={request?.ManagerID}, StoreID={request?.StoreID}, Rate={request?.Rate}, TimeIn={request?.TimeIn}");

            if (request == null || request.ManagerID <= 0 || request.StoreID <= 0)
            {
                return BadRequest("Invalid input data.");
            }

            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = @"INSERT INTO [TimeClock].[TimeManagement] (ManagerID, StoreID, Rate, ServerTimeIn, ClockedIn, Approved) 
                            VALUES (?, ?, ?, ?, 1, 0)";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.Add("?", OdbcType.Int).Value = request.ManagerID;
                        command.Parameters.Add("?", OdbcType.Int).Value = request.StoreID;
                        command.Parameters.Add("?", OdbcType.Decimal).Value = request.Rate;
                        command.Parameters.Add("?", OdbcType.DateTime).Value = request.TimeIn;

                        int result = await command.ExecuteNonQueryAsync();
                        _logger.LogInformation($"Clock-In Query Executed: {result} row(s) affected.");

                        return result > 0 ? Ok("Clock-in recorded successfully.") : StatusCode(409, "Clock-in already exists or failed to insert.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during clock-in: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("clockout")]
        public async Task<IActionResult> ClockOut([FromBody] TimePunchRequest request)
        {
            _logger.LogInformation($"Clock-Out Request Received: ManagerID={request?.ManagerID}");

            if (request == null || request.ManagerID <= 0)
            {
                return BadRequest("Invalid input data.");
            }

            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = @"UPDATE [TimeClock].[TimeManagement] 
                             SET ServerTimeOut = ?, ClockedIn = 0 
                             WHERE ManagerID = ? AND ServerTimeOut IS NULL";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.Add("?", OdbcType.DateTime).Value = DateTime.UtcNow;
                        command.Parameters.Add("?", OdbcType.Int).Value = request.ManagerID;

                        int result = await command.ExecuteNonQueryAsync();
                        _logger.LogInformation($"Clock-Out Query Executed: {result} row(s) affected.");

                        return result > 0 ? Ok("Clock-out recorded.") : StatusCode(409, "No active clock-in found.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during clock-out: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<List<TimeClockEntry>> GetTimeClockEntries(DateTime startDate, DateTime endDate, int employeeId)
        {
            var timeClockEntries = new List<TimeClockEntry>();
            try
            {
                using (var conn = new OdbcConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    StringBuilder query = new StringBuilder(@"SELECT TM.TMID, TM.ManagerID, 
                COALESCE(M.FirstName, '') + ' ' + COALESCE(M.LastName, '') AS EmployeeName, 
                TM.StoreID, TM.Rate, TM.ServerTimeIn, TM.ServerTimeOut, TM.Approved, TM.ApprovedBy, 
                COALESCE(A.FirstName, '') + ' ' + COALESCE(A.LastName, '') AS ApprovedByName 
                FROM [TimeClock].[TimeManagement] TM 
                LEFT JOIN [Manager].[tblManagers] M ON TM.ManagerID = M.ManagerID 
                LEFT JOIN [Manager].[tblManagers] A ON TM.ApprovedBy = A.ManagerID 
                WHERE TM.ManagerID = ? 
                AND TM.ServerTimeIn >= ? 
                AND TM.ServerTimeIn <= ?");

                    using (var cmd = new OdbcCommand(query.ToString(), conn))
                    {
                        cmd.Parameters.AddWithValue("?", employeeId);
                        cmd.Parameters.AddWithValue("?", startDate);
                        cmd.Parameters.AddWithValue("?", endDate);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                timeClockEntries.Add(new TimeClockEntry
                                {
                                    TMID = reader.GetInt32(0),
                                    ManagerID = reader.GetInt32(1),
                                    EmployeeName = reader.GetString(2),
                                    StoreID = reader.GetInt32(3),
                                    Rate = reader.GetDecimal(4),
                                    ServerTimeIn = reader.GetDateTime(5),
                                    ServerTimeOut = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                    Approved = reader.GetBoolean(7),
                                    ApprovedBy = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                                    ApprovedByName = reader.GetString(9)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching entries: {ex.Message}");
            }
            return timeClockEntries;

        }


    }

}

