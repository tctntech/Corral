namespace Corral.Models
{
    public class VendorExpenses
    {
        public int StoreidExp { get; set; }
        public int VendorYear { get; set; }
        public decimal VendorExpense { get; set; }
        public string Vendor { get; set; }
        public decimal Sales { get; set; }
        public decimal ExpensePercent { get; set; }
    }
}
