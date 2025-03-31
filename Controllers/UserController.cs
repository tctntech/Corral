using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Threading.Tasks;
using Corral.Models;

namespace Corral.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        private readonly string connectionString = "Driver={ODBC Driver 17 for SQL Server};Server=192.168.5.15;Database=TexasCorralQA;Uid=zpatel;Pwd=@TCzp2219!;";
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Fetch list of users with optional filter (active/inactive)
        [HttpGet("list")]
        public JsonResult GetUsers(string filter = "")
        {
            List<ManagerUser> managers = new List<ManagerUser>();
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ManagerID, FullName, Username, Active, HomeStore, AccountActiveDate FROM [Manager].[tblManagers]";

                    if (filter == "active")
                        query += " WHERE Active = '1'";
                    else if (filter == "inactive")
                        query += " WHERE Active = '0'";

                    using (var command = new OdbcCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            managers.Add(new ManagerUser
                            {
                                ManagerID = reader.GetInt32(0),
                                FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Username = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Active = reader.IsDBNull(3) ? "0" : reader.GetString(3).Trim(),
                                HomeStore = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                AccountActiveDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching users: {ex.Message}");
                return Json(new { success = false, message = "Error fetching users." });
            }
            return Json(new { success = true, data = managers });
        }

        // Fetch user details by ManagerID
        [HttpGet("details/{id}")]
        public JsonResult GetUserDetails(int id)
        {
            ManagerUser manager = null;
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ManagerID, FullName, Username, Active, HomeStore, AccountActiveDate,FirstName,[Middle Name],[LastName],[TimeClockID],[SecurityLevel],[AccountExpireDate],[TimeClockPassCode],[AllowMultiStore],[AllowIntranetAccess]  FROM [Manager].[tblManagers] WHERE ManagerID = ?";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                manager = new ManagerUser
                                {
                                    ManagerID = reader.GetInt32(0),
                                    FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    Username = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                   FirstName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                    MiddleName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                    LastName = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                    TimeClockID = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                    Active = reader.IsDBNull(3) ? "0" : reader.GetString(3).Trim(),
                                    HomeStore = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                    SecurityLevel = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                                    AccountActiveDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                    AccountExpireDate = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),
                                    TimeClockPassCode = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                    AllowMultiStore = reader.IsDBNull(13) ? 0 : reader.GetDecimal(13),
                                    AllowIntranetAccess = reader.IsDBNull(14) ? 0 : reader.GetDecimal(14)





                                };
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error fetching user details: {ex.Message}");
                return Json(new { success = false, message = "Error fetching user details." });
            }
            return Json(new { success = true, data = manager });
        }

        // Update user details
        [HttpPost("update")]
        public JsonResult UpdateUser([FromBody] ManagerUser manager)
        {
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE [Manager].[tblManagers]
                        SET FullName = ?, Username = ?, Active = ?, HomeStore = ?, AccountActiveDate = ?
                        WHERE ManagerID = ?";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", manager.FullName);
                        command.Parameters.AddWithValue("?", manager.Username);
                        command.Parameters.AddWithValue("?", manager.Active);
                        command.Parameters.AddWithValue("?", manager.HomeStore ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("?", manager.AccountActiveDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("?", manager.ManagerID);

                        int rowsAffected = command.ExecuteNonQuery();
                        return Json(new { success = rowsAffected > 0, message = rowsAffected > 0 ? "User updated successfully." : "No records updated." });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user: {ex.Message}");
                return Json(new { success = false, message = "Error updating user." });
            }
        }
        // Fetch permission access for a given ManagerID
        [HttpGet("permissions/{managerID}")]
        public JsonResult GetUserPermissions(int managerID)
        {
            List<ManagerPermission> permissions = new List<ManagerPermission>();
            try
            {
                // Validate the ManagerID
                if (managerID <= 0)
                {
                    return Json(new { success = false, message = "Invalid ManagerID.", data = permissions });
                }

                _logger.LogInformation($"Attempting to fetch permissions for ManagerID: {managerID}");

                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Database connection opened.");

                    // SQL query to fetch permissions assigned to the given ManagerID
                    string query = @"
                SELECT pa.PermissionID, 
                       pa.PermissionName, 
                       pa.PermissionDescription, 
                       pa.FoundInModule,
                       pa2.ManagerID
                FROM [Manager].[PermissionsAvailable] pa
                INNER JOIN [Manager].[PermissionsAssigned] pa2
                    ON pa.PermissionID = pa2.PermissionID
                WHERE pa2.ManagerID = ?";

                    _logger.LogInformation($"Query: {query}, ManagerID: {managerID}");

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", managerID);
                        _logger.LogInformation("Command parameters set.");

                        using (var reader = command.ExecuteReader())
                        {
                            _logger.LogInformation("Executing query...");
                            while (reader.Read())
                            {
                                permissions.Add(new ManagerPermission
                                {
                                    PermissionID = reader.GetInt32(0),
                                    PermissionName = reader.GetString(1),
                                    PermissionDescription = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                    FoundInModule = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                    ManagerID = reader.GetInt32(4)
                                });
                            }
                        }
                    }
                }

                // If no permissions are found, return a message
                if (permissions.Count == 0)
                {
                    return Json(new { success = false, message = "No permissions found for this manager.", data = permissions });
                }

                return Json(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching permissions for ManagerID {managerID}: {ex.Message}");
                return Json(new { success = false, message = ex.Message, data = new List<ManagerPermission>() });
            }
        }

        // Fetch module access for a given ManagerID
        [HttpGet("modules/{managerID}")]
        public JsonResult GetUserModuleAccess(int managerID)
        {
            List<object> modules = new List<object>();
            try
            {
                // Validate the ManagerID
                if (managerID <= 0)
                {
                    return Json(new { success = false, message = "Invalid ManagerID.", data = modules });
                }

                _logger.LogInformation($"Attempting to fetch modules for ManagerID: {managerID}");

                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Database connection opened.");

                    // SQL query to fetch modules
                    string query = @"
                SELECT mp.ManagerID, pm.ModuleID, pm.ModuleName, pm.ModuleDescription, pm.ModuleForm, pm.ModuleParent
                FROM [Manager].[ModulePermissions] mp
                INNER JOIN [General].[ProgramModules] pm ON mp.ModuleID = pm.ModuleID
                WHERE mp.ManagerID = ?";

                    _logger.LogInformation($"Query: {query}, ManagerID: {managerID}");

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", managerID);
                        _logger.LogInformation("Command parameters set.");

                        using (var reader = command.ExecuteReader())
                        {
                            _logger.LogInformation("Executing query...");
                            while (reader.Read())
                            {
                                modules.Add(new
                                {
                                    ManagerID = reader.GetInt32(0),
                                    ModuleID = reader.GetInt32(1),
                                    ModuleName = reader.GetString(2),
                                    ModuleParent = reader.IsDBNull(5) ? null : reader.GetString(5),

                                    ModuleDescription = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                    ModuleForm = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),

                                });
                            }
                        }
                    }
                }

                // If no modules are found, return a message
                if (modules.Count == 0)
                {
                    return Json(new { success = false, message = "No modules found for this manager.", data = modules });
                }

                return Json(new { success = true, data = modules });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching modules for ManagerID {managerID}: {ex.Message}");
                return Json(new { success = false, message = ex.Message, data = new List<object>() });
            }
        }

    }

}