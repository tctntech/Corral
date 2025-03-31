namespace Corral.Models
{
    public class ProductEventHistory
    {
        public string StoreID { get; set; } // nvarchar(100)
        public string TransferToStoreID { get; set; } // nvarchar(100)
        public DateTime EntryDate { get; set; } // date
        public int ItemID { get; set; } // int
        public string ItemName { get; set; } // nvarchar(100)
        public string EntryType { get; set; } // nvarchar(100)
        public int EventTypeID { get; set; } // int
        public int EventRef { get; set; } // int
        public string EntryName { get; set; } // nvarchar(100)
        public decimal Qty { get; set; } // numeric(18,6)
        public string InventoryUnits { get; set; } // nvarchar(100)
        public decimal InventoryUnitCost { get; set; } // numeric(18,6)
        public decimal TotalCost { get; set; } // money (decimal(19,4))
    }
}
