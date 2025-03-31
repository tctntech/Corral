namespace Corral.Models
{
    public class StoreProducts
    {
        public int StoreProductID { get; set; }
        public string StoreID { get; set; }
        public int ProductID { get; set; }
        public DateTime? DateAdded { get; set; }
        public int? InventoryWeekly { get; set; }
        public int? InventoryDaily { get; set; }
        public int? InventoryUnit { get; set; }
        public int? Alert { get; set; }
        public decimal? AlertPrice { get; set; }
        public int? AlertUnits { get; set; }
        public decimal? Active { get; set; }
    }
}
