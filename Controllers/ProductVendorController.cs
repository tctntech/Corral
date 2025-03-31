using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Globalization;
using System.Threading.Tasks;
using Corral.Models;
using iText.StyledXmlParser.Jsoup.Select;
using System.Reflection;


namespace Corral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVendorController : ControllerBase
    {
        private readonly string connectionString = "Driver={ODBC Driver 17 for SQL Server};Server=192.168.5.15;Database=TexasCorralQA;Uid=zpatel;Pwd=@TCzp2219!;Trusted_Connection=no";

        // 1. Get All Absolute Products
        [HttpGet("GetAll")]
        public IActionResult GetAllAbsoluteProducts()
        {
            List<ProductAbsolute> products = new List<ProductAbsolute>();

            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                string query = "SELECT p.ProductID, p.ProductName, p.ProductDescription, p.BrandID, b.BrandName, p.BaseTareWeight, p.RelativeProductID, rp.RProductName AS RelativeProductName, p.BaseContainerID, c.ContainerName,  p.Notes, p.LinkedToRelative, p.ExplicitCategoryID, p.Inventoried, p.Asset, p.InventoryUnitCtTxt, p.InventoryCtQty, p.InventoryCtUnits, p.DateCreated, p.CreatedByManagerID, p.ItemCostCanBeModifiedInTransfer FROM Products.AbsoluteProducts p INNER JOIN Products.Brands b ON p.BrandID = b.BrandID LEFT JOIN Products.RelativeProducts rp ON p.RelativeProductID = rp.RProductID LEFT JOIN Products.lkpBaseContainer c ON p.BaseContainerID = c.ContainerID;";

                OdbcCommand cmd = new OdbcCommand(query, conn);

                conn.Open();
                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(MapProductAbsolute(reader));
                    }
                }
            }

            return Ok(products);
        }





        [HttpGet("GetProductEventHistory")]
        public async Task<IActionResult> GetProductEventHistory(int itemId, string bdate, string edate)
        {
            try
            {
                // Validate and convert date strings to DateTime
                if (!DateTime.TryParseExact(bdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) ||
                    !DateTime.TryParseExact(edate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
                {
                    return BadRequest("Invalid date format. Use YYYY-MM-DD.");
                }

                string query = "SELECT * FROM dbo.tv_ProductsAbsoluteAllEventsHistoryBetweenDates (?, ?, ?)";

                List<ProductEventHistory> historyList = new List<ProductEventHistory>();

                using (OdbcConnection conn = new OdbcConnection(connectionString))
                using (OdbcCommand cmd = new OdbcCommand(query, conn))
                {
                    // Set SQL parameters
                    cmd.Parameters.Add("?", OdbcType.Int).Value = itemId;
                    cmd.Parameters.Add("?", OdbcType.DateTime).Value = startDate;
                    cmd.Parameters.Add("?", OdbcType.DateTime).Value = endDate;

                    conn.Open();
                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            historyList.Add(new ProductEventHistory
                            {
                                StoreID = reader.GetString(0),
                                TransferToStoreID = reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                ItemID = reader.GetInt32(3),
                                ItemName = reader.GetString(4),
                                EntryType = reader.GetString(5),
                                EventTypeID = reader.GetInt32(6),
                                EventRef = reader.GetInt32(7),
                                EntryName = reader.GetString(8),
                                Qty = reader.GetDecimal(9),
                                InventoryUnits = reader.GetString(10),
                                InventoryUnitCost = reader.GetDecimal(11),
                                TotalCost = reader.GetDecimal(12)
                            });
                        }
                    }
                }

                return Ok(historyList);
            }
            catch (OdbcException odbcEx)
            {
                return StatusCode(500, $"ODBC Error: {odbcEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        // 2. Get Product Details by ID
        [HttpGet("GetById/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            ProductAbsolute product = null;

            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {

                string query = "SELECT p.ProductID, p.ProductName, p.ProductDescription, p.BrandID, b.BrandName, p.BaseTareWeight, p.RelativeProductID, rp.RProductName AS RelativeProductName, p.BaseContainerID, c.ContainerName,  p.Notes, p.LinkedToRelative, p.ExplicitCategoryID, p.Inventoried, p.Asset, p.InventoryUnitCtTxt, p.InventoryCtQty, p.InventoryCtUnits, p.DateCreated, p.CreatedByManagerID, p.ItemCostCanBeModifiedInTransfer FROM Products.AbsoluteProducts p INNER JOIN Products.Brands b ON p.BrandID = b.BrandID LEFT JOIN Products.RelativeProducts rp ON p.RelativeProductID = rp.RProductID LEFT JOIN Products.lkpBaseContainer c ON p.BaseContainerID = c.ContainerID  WHERE p.ProductID = ?;";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("?", OdbcType.Int).Value = productId;  // Fixed parameter binding

                conn.Open();
                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = MapProductAbsolute(reader);
                    }
                }
            }

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        // 3. Update Absolute Product
        [HttpPut("UpdateAbsoluteProduct/{productId}")]
        public async Task<IActionResult> UpdateAbsoluteProduct(int productId, [FromBody] ProductAbsolute updatedProduct)
        {
            if (updatedProduct == null || productId != updatedProduct.ProductID)
            {
                return BadRequest("Invalid product data.");
            }

            string query = @"
    UPDATE Products.AbsoluteProducts
    SET 
        ProductName = ?, 
        ProductDescription = ?, 
        BrandID = ?, 
        BaseContainerID = ?, 
        Asset = ?, 
        ItemCostCanBeModifiedInTransfer = ?, 
        Inventoried = ?, 
        BaseTareWeight = ?, 
        InventoryUnitCtTxt = ?, 
        InventoryCtQty = ?, 
        InventoryCtUnits = ?, 
        LinkedToRelative = ?, 
        RelativeProductID = ?, 
        ExplicitCategoryID = ?
    WHERE ProductID = ?";

            try
            {
                using (OdbcConnection conn = new OdbcConnection(connectionString))
                using (OdbcCommand cmd = new OdbcCommand(query, conn))
                {
                    BindProductParameters(cmd, updatedProduct);
                    cmd.Parameters.Add("?", OdbcType.Int).Value = productId;  // Last parameter for WHERE clause

                    conn.Open();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                        return Ok("Product updated successfully.");
                    else
                        return NotFound("Product not found.");
                }
            }
            catch (OdbcException odbcEx)
            {
                return StatusCode(500, $"ODBC Error: {odbcEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        // 4. Get Relative Product Valid Units
        [HttpGet("GetRelativeProductValidUnits/{relativeProductID}")]
        public IActionResult GetRelativeProductValidUnits(int relativeProductID)
        {
            try
            {
                List<ProductValidUnit> result = new List<ProductValidUnit>();

                using (OdbcConnection conn = new OdbcConnection(connectionString))  // Fixed connection string variable
                {
                    conn.Open();
                    string query = "SELECT * FROM dbo.tv_ProductsRelativeProductValidUnits(?)";

                    using (OdbcCommand cmd = new OdbcCommand(query, conn))
                    {
                        cmd.Parameters.Add("?", OdbcType.Int).Value = relativeProductID;  // Fixed parameter binding

                        using (OdbcDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new ProductValidUnit
                                {
                                    UnitID = reader.GetInt32(0),
                                    UnitName = reader.GetString(1),
                                    UnitAbr = reader.GetString(2),
                                    Form = reader.GetString(3),
                                    AbsoluteMeasurement = reader.GetString(4),
                                    IsBase = reader.GetString(5) // Keep as string ("0" or "1")
                                });
                            }
                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }







        private ProductAbsolute MapProductAbsolute(OdbcDataReader reader)
        {
            return new ProductAbsolute
            {
                ProductID = reader.GetInt32(0), // INT - Not Nullable
                ProductName = reader.IsDBNull(1) ? null : reader.GetString(1), // NVARCHAR
                ProductDescription = reader.IsDBNull(2) ? null : reader.GetString(2), // NVARCHAR
                BrandID = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3), // INT - Nullable
                BrandName = reader.IsDBNull(4) ? null : reader.GetString(4), // NVARCHAR
                BaseTareWeight = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5), // NUMERIC - Nullable
                RelativeProductID = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6), // INT - Nullable
                RelativeProductName = reader.IsDBNull(7) ? null : reader.GetString(7), // NVARCHAR
                BaseContainerID = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8), // INT - Nullable
                ContainerName = reader.IsDBNull(9) ? null : reader.GetString(9), // NVARCHAR
                Notes = reader.IsDBNull(10) ? null : reader.GetString(10), // NVARCHAR
                LinkedToRelative = reader.IsDBNull(11) ? false : reader.GetDecimal(11) == 1m, // NUMERIC to BOOL
                ExplicitCategoryID = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12), // INT - Nullable
                Inventoried = reader.IsDBNull(13) ? false : reader.GetDecimal(13) == 1m, // NUMERIC to BOOL
                Asset = reader.IsDBNull(14) ? false : reader.GetDecimal(14) == 1m, // NUMERIC to BOOL
                InventoryUnitCtTxt = reader.IsDBNull(15) ? null : reader.GetString(15), // NVARCHAR
                InventoryCtQty = reader.IsDBNull(16) ? (decimal?)null : reader.GetDecimal(16), // NUMERIC - Nullable
                InventoryCtUnits = reader.IsDBNull(17) ? (int?)null : reader.GetInt32(17), // INT - Nullable
                DateCreated = reader.IsDBNull(18) ? (DateTime?)null : reader.GetDateTime(18), // DATETIME - Nullable
                CreatedByManagerID = reader.IsDBNull(19) ? (int?)null : reader.GetInt32(19), // INT - Nullable
                ItemCostCanBeModifiedInTransfer = reader.IsDBNull(20) ? false : reader.GetBoolean(20) // BIT to BOOL

            };
        }






        private decimal? GetNullableDecimal(OdbcDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (decimal?)null : reader.GetDecimal(index);
        }







        // Helper function to bind product parameters
        private void BindProductParameters(OdbcCommand cmd, ProductAbsolute product)
        {
            cmd.Parameters.Add("?", OdbcType.NVarChar, 250).Value = product.ProductName ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.NVarChar, -1).Value = product.ProductDescription ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.Int).Value = product.BrandID ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.Int).Value = product.BaseContainerID ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.Bit).Value = product.Asset;
            cmd.Parameters.Add("?", OdbcType.Bit).Value = product.ItemCostCanBeModifiedInTransfer;
            cmd.Parameters.Add("?", OdbcType.Bit).Value = product.Inventoried;
            cmd.Parameters.Add("?", OdbcType.Numeric).Value = product.BaseTareWeight ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.NVarChar, 50).Value = product.InventoryUnitCtTxt ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.Decimal).Value = product.InventoryCtQty ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.Int).Value = product.InventoryCtUnits ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.Bit).Value = product.LinkedToRelative;
            cmd.Parameters.Add("?", OdbcType.Int).Value = product.RelativeProductID ?? (object)DBNull.Value;
            cmd.Parameters.Add("?", OdbcType.Int).Value = product.ExplicitCategoryID ?? (object)DBNull.Value;
        }


    }
}
