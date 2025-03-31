using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Threading.Tasks;
using Corral.Models;


namespace Corral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly string connectionString = "Driver={ODBC Driver 17 for SQL Server};Server=192.168.5.15;Database=TexasCorralQA;Uid=zpatel;Pwd=@TCzp2219!;Trusted_Connection=no";

        // 1. Get All Vendors
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllVendors()
        {
            List<Vendor> vendors = new List<Vendor>();

            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                String query = "SELECT v.VendorID,v.VendorName, \r\n    v.ContactName, \r\n    v.Address, \r\n    v.City, \r\n    v.State, \r\n    v.Zip, \r\n    v.Phone, \r\n    v.Fax, \r\n    v.Terms, \r\n    v.CalculateDueDateWithTerms, \r\n    v.OurAccountNumber, \r\n    v.Comments, \r\n    v.Active, \r\n    v.Website, \r\n    v.Email, \r\n    v.ProductsProvided, \r\n    v.GlobalVendor, \r\n    v.DateAdded, \r\n    v.AddedBy, \r\n    v.MaxDaysToAllowFuturePost,\r\n    sve.ExclusionID,\r\n    sve.StoreID AS ExclusionStoreID,\r\n    si.City AS ExclusionStoreCity,\r\n    svi.InclusionID,\r\n    svi.StoreID AS InclusionStoreID,\r\n    si2.City AS InclusionStoreCity\r\nFROM [TexasCorralQA].[Products].[AllVendors] v\r\nLEFT JOIN [TexasCorralQA].[Products].[StoreVendorExclusions] sve \r\n    ON v.VendorID = sve.VendorID\r\nLEFT JOIN [TexasCorralQA].[General].[StoreInformation] si \r\n    ON sve.StoreID = si.StoreNumber\r\nLEFT JOIN [TexasCorralQA].[Products].[StoreVendorInclusion] svi\r\n    ON v.VendorID = svi.VendorID\r\nLEFT JOIN [TexasCorralQA].[General].[StoreInformation] si2 \r\n    ON svi.StoreID = si2.StoreNumber\r\nORDER BY v.VendorID;\r\n";
                OdbcCommand cmd = new OdbcCommand(query, conn);

                conn.Open();
                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vendors.Add(new Vendor
                        {
                            VendorID = reader.GetInt32(0),
                            VendorName = reader.GetString(1),
                            ContactName = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Address = reader.IsDBNull(3) ? null : reader.GetString(3),
                            City = reader.IsDBNull(4) ? null : reader.GetString(4),
                            State = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Zip = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Phone = reader.IsDBNull(7) ? null : reader.GetString(7),
                            Fax = reader.IsDBNull(8) ? null : reader.GetString(8),
                            Terms = reader.IsDBNull(9) ? null : reader.GetString(9),
                            CalculateDueDateWithTerms = reader.IsDBNull(10) ? (bool?)null : reader.GetBoolean(10),
                            OurAccountNumber = reader.IsDBNull(11) ? null : reader.GetString(11),
                            Comments = reader.IsDBNull(12) ? null : reader.GetString(12),

                            // Convert Active from nchar ('0'/'1') to bool?
                            Active = reader.IsDBNull(13) ? (bool?)null : reader.GetString(13).Trim() == "1",

                            Website = reader.IsDBNull(14) ? null : reader.GetString(14),
                            Email = reader.IsDBNull(15) ? null : reader.GetString(15),
                            ProductsProvided = reader.IsDBNull(16) ? null : reader.GetString(16),
                            GlobalVendor = reader.IsDBNull(17) ? (decimal?)null : reader.GetDecimal(17),
                            DateAdded = reader.IsDBNull(18) ? (DateTime?)null : reader.GetDateTime(18),
                            AddedBy = reader.IsDBNull(19) ? (int?)null : reader.GetInt32(19),
                            MaxDaysToAllowFuturePost = reader.IsDBNull(20) ? (int?)null : reader.GetInt32(20),

                            // Exclusion details
                            ExclusionID = reader.IsDBNull(21) ? (int?)null : reader.GetInt32(21),
                            ExclusionStoreID = reader.IsDBNull(22) ? null : reader.GetString(22), // Changed to string
                            ExclusionStoreCity = reader.IsDBNull(23) ? null : reader.GetString(23),

                            // Inclusion details
                            InclusionID = reader.IsDBNull(24) ? (int?)null : reader.GetInt32(24),
                            InclusionStoreID = reader.IsDBNull(25) ? null : reader.GetString(25), // Changed to string
                            InclusionStoreCity = reader.IsDBNull(26) ? null : reader.GetString(26),
                        });
                    }
                }
            }
            return Ok(vendors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorById(int id)
        {
            try
            {
                Vendor vendor = null;
                List<StoreVendorExclusions> exclusions = new List<StoreVendorExclusions>();
                List<StoreVendorInclusion> inclusions = new List<StoreVendorInclusion>();

                // Open connection and run the query to fetch vendor data along with exclusions and inclusions
                using (OdbcConnection conn = new OdbcConnection(connectionString))
                {
                    conn.Open();

                    // Query to fetch vendor details along with exclusions and inclusions
                    using (OdbcCommand cmd = new OdbcCommand(@"
                SELECT 
                    v.VendorID, 
                    v.VendorName, 
                    v.ContactName, 
                    v.Address, 
                    v.City, 
                    v.State, 
                    v.Zip, 
                    v.Phone, 
                    v.Fax, 
                    v.Terms, 
                    v.CalculateDueDateWithTerms, 
                    v.OurAccountNumber, 
                    v.Comments, 
                    v.Active, 
                    v.Website, 
                    v.Email, 
                    v.ProductsProvided, 
                    v.GlobalVendor, 
                    v.DateAdded, 
                    v.AddedBy, 
                    v.MaxDaysToAllowFuturePost,
                    sve.ExclusionID,
                    sve.StoreID AS ExclusionStoreID,
                    si.City AS ExclusionStoreCity,
                    svi.InclusionID,
                    svi.StoreID AS InclusionStoreID,
                    si2.City AS InclusionStoreCity
                FROM [TexasCorralQA].[Products].[AllVendors] v
                LEFT JOIN [TexasCorralQA].[Products].[StoreVendorExclusions] sve 
                    ON v.VendorID = sve.VendorID
                LEFT JOIN [TexasCorralQA].[General].[StoreInformation] si 
                    ON sve.StoreID = si.StoreNumber
                LEFT JOIN [TexasCorralQA].[Products].[StoreVendorInclusion] svi
                    ON v.VendorID = svi.VendorID
                LEFT JOIN [TexasCorralQA].[General].[StoreInformation] si2 
                    ON svi.StoreID = si2.StoreNumber
                WHERE v.VendorID = ?", conn))
                    {
                        cmd.Parameters.AddWithValue("VendorID", id);

                        using (OdbcDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Populate vendor details
                                if (vendor == null)
                                {
                                    vendor = new Vendor
                                    {
                                        VendorID = reader.GetInt32(reader.GetOrdinal("VendorID")),
                                        VendorName = reader.IsDBNull(reader.GetOrdinal("VendorName")) ? null : reader.GetString(reader.GetOrdinal("VendorName")),
                                        ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? null : reader.GetString(reader.GetOrdinal("ContactName")),
                                        Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                                        City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader.GetString(reader.GetOrdinal("City")),
                                        State = reader.IsDBNull(reader.GetOrdinal("State")) ? null : reader.GetString(reader.GetOrdinal("State")),
                                        Zip = reader.IsDBNull(reader.GetOrdinal("Zip")) ? null : reader.GetString(reader.GetOrdinal("Zip")),
                                        Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                                        Fax = reader.IsDBNull(reader.GetOrdinal("Fax")) ? null : reader.GetString(reader.GetOrdinal("Fax")),
                                        Terms = reader.IsDBNull(reader.GetOrdinal("Terms")) ? null : reader.GetString(reader.GetOrdinal("Terms")),
                                        CalculateDueDateWithTerms = reader.IsDBNull(reader.GetOrdinal("CalculateDueDateWithTerms")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("CalculateDueDateWithTerms")),
                                        OurAccountNumber = reader.IsDBNull(reader.GetOrdinal("OurAccountNumber")) ? null : reader.GetString(reader.GetOrdinal("OurAccountNumber")),
                                        Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                                        Active = reader.IsDBNull(reader.GetOrdinal("Active")) ? (bool?)null : reader.GetString(reader.GetOrdinal("Active")).Trim() == "1",

                                        Website = reader.IsDBNull(reader.GetOrdinal("Website")) ? null : reader.GetString(reader.GetOrdinal("Website")),
                                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                        ProductsProvided = reader.IsDBNull(reader.GetOrdinal("ProductsProvided")) ? null : reader.GetString(reader.GetOrdinal("ProductsProvided")),
                                        GlobalVendor = reader.IsDBNull(reader.GetOrdinal("GlobalVendor")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("GlobalVendor")),
                                        DateAdded = reader.IsDBNull(reader.GetOrdinal("DateAdded")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                                        AddedBy = reader.IsDBNull(reader.GetOrdinal("AddedBy")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AddedBy")),
                                        MaxDaysToAllowFuturePost = reader.IsDBNull(reader.GetOrdinal("MaxDaysToAllowFuturePost")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("MaxDaysToAllowFuturePost"))
                                    };
                                }

                                // Add exclusions if available
                                if (!reader.IsDBNull(reader.GetOrdinal("ExclusionID")))
                                {
                                    exclusions.Add(new StoreVendorExclusions
                                    {
                                        ExclusionID = reader.GetInt32(reader.GetOrdinal("ExclusionID")),
                                        StoreID = reader.GetString(reader.GetOrdinal("ExclusionStoreID")),
                                        ExclusionStoreCity = reader.IsDBNull(reader.GetOrdinal("ExclusionStoreCity")) ? null : reader.GetString(reader.GetOrdinal("ExclusionStoreCity"))
                                    });
                                }

                                // Add inclusions if available
                                if (!reader.IsDBNull(reader.GetOrdinal("InclusionID")))
                                {
                                    inclusions.Add(new StoreVendorInclusion
                                    {
                                        InclusionID = reader.GetInt32(reader.GetOrdinal("InclusionID")),
                                        StoreID = reader.GetString(reader.GetOrdinal("InclusionStoreID")),
                                        InclusionStoreCity = reader.IsDBNull(reader.GetOrdinal("InclusionStoreCity")) ? null : reader.GetString(reader.GetOrdinal("InclusionStoreCity"))
                                    });
                                }
                            }
                        }
                    }
                }

                // If no vendor found, return NotFound
                if (vendor == null)
                {
                    return NotFound();
                }

                // Add exclusions and inclusions to vendor object
                vendor.StoreVendorExclusions = exclusions;
                vendor.StoreVendorInclusions = inclusions;

                // Return the vendor object with the associated exclusions and inclusions
                return Ok(vendor);
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetAllStoreVendors")]
        public async Task<IActionResult> GetAllStoreVendors()
        {
            List<StoreVendors> storeVendors = new List<StoreVendors>();

            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                string query = "SELECT StoreVendorID, StoreID, VendorID, DateAdded, Active FROM [TexasCorralQA].[Products].[StoreVendors]";
                OdbcCommand cmd = new OdbcCommand(query, conn);

                conn.Open();
                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storeVendors.Add(new StoreVendors
                        {
                            StoreVendorID = reader.GetInt32(0),
                            StoreID = reader.GetString(1),
                            VendorID = reader.GetInt32(2),
                            DateAdded = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            Active = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4)
                        });
                    }
                }
            }
            return Ok(storeVendors);
        }
       
        [HttpPost]
        public async Task<IActionResult> CreateVendor([FromBody] Vendor vendor)
        {
            if (vendor == null)
            {
                return BadRequest("Vendor data is required.");
            }

            // Create the insert query
            string query = @"
        INSERT INTO [TexasCorralQA].[Products].[AllVendors]
        (VendorName, ContactName, Address, City, State, Zip, Phone, Fax, Terms, 
         CalculateDueDateWithTerms, OurAccountNumber, Comments, Active, Website, Email, 
         ProductsProvided, GlobalVendor, DateAdded, AddedBy, MaxDaysToAllowFuturePost)
        VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.AddWithValue("@VendorName", vendor.VendorName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactName", vendor.ContactName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", vendor.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", vendor.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", vendor.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Zip", vendor.Zip ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", vendor.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Fax", vendor.Fax ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Terms", vendor.Terms ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CalculateDueDateWithTerms", vendor.CalculateDueDateWithTerms ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OurAccountNumber", vendor.OurAccountNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Comments", vendor.Comments ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Active", vendor.Active);
                cmd.Parameters.AddWithValue("@Website", vendor.Website ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", vendor.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductsProvided", vendor.ProductsProvided ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GlobalVendor", vendor.GlobalVendor ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                cmd.Parameters.AddWithValue("@AddedBy", vendor.AddedBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxDaysToAllowFuturePost", vendor.MaxDaysToAllowFuturePost ?? (object)DBNull.Value);

                conn.Open();
                await cmd.ExecuteNonQueryAsync();
            }

            return Ok(vendor); // Return the created vendor object (can also return 201 Created if needed)
        }
        [HttpGet("GetAllStoreExpenses")]
        public async Task<IActionResult> GetAllStoreExpenses()
        {
            List<VendorExpenses> storeExpenses = new List<VendorExpenses>();

            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                // Fixed the query syntax by removing the extra "]"
                string query = "SELECT StoreidExp, VendorYear, VendorExpense, Vendor, Sales, ExpensePercent FROM [TexasCorralQA].[DW].[VendorExpense]";
                OdbcCommand cmd = new OdbcCommand(query, conn);

                conn.Open();
                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storeExpenses.Add(new VendorExpenses
                        {
                            StoreidExp = reader.GetInt32(0),
                            VendorYear = reader.GetInt32(1),
                            VendorExpense = reader.GetDecimal(2),
                            Vendor = reader.GetString(3), // Make sure the Vendor field is a string
                            Sales = reader.GetDecimal(4),
                            ExpensePercent = reader.GetDecimal(5)
                        });
                    }
                }
            }
            return Ok(storeExpenses);
        }



    }
}





