namespace Corral.Models
{
    public class ProductAbsolute
    {
        public int ProductID { get; set; } // INT - Not Nullable
        public string? ProductName { get; set; } // NVARCHAR - Nullable
        public string? ProductDescription { get; set; } // NVARCHAR - Nullable
        public int? BrandID { get; set; } // INT - Nullable
        public string? BrandName { get; set; } // NVARCHAR - Nullable

        // NUMERIC (DECIMAL) - Nullable
        public decimal? BaseTareWeight { get; set; }

        public int? RelativeProductID { get; set; } // INT - Nullable

        // INT - Nullable
        public int? BaseContainerID { get; set; } // INT - Nullable

        public string? Notes { get; set; } // NVARCHAR - Nullable

        // NUMERIC (DECIMAL) - Convert to BOOL (0 or 1)
        public bool LinkedToRelative { get; set; }
        public string? RelativeProductName { get; set; }

        public int? ExplicitCategoryID { get; set; } // INT - Nullable

        // NUMERIC (DECIMAL) - Convert to BOOL (0 or 1)
        public bool Inventoried { get; set; }

        // NUMERIC (DECIMAL) - Convert to BOOL (0 or 1)
        public bool Asset { get; set; }

        public string? InventoryUnitCtTxt { get; set; } // NVARCHAR - Nullable

        // NUMERIC (DECIMAL) - Nullable
        public decimal? InventoryCtQty { get; set; }

        // INT - Nullable
        public int? InventoryCtUnits { get; set; } // INT - Nullable

        // DATETIME - Nullable
        public DateTime? DateCreated { get; set; }

        // INT - Nullable
        public int? CreatedByManagerID { get; set; }

        // BIT - Use `bool`
        public bool ItemCostCanBeModifiedInTransfer { get; set; }
        public string? ContainerName { get; set; }
    }
}
