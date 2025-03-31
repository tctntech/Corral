namespace Corral.Models
{
    public class VendorProduct
    {
        public int VendorID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; } // Vendor-specific price for the product
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
