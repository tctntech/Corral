namespace Corral.Models
{
    public class SalaryDiscrepancy
    {
        public int Storeid { get; set; }
        public int Staffuid { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Payrollid { get; set; }
        public decimal ActualWorked { get; set; }
        public decimal ActPaidHours { get; set; }
        public decimal Difference { get; set; }
    }

}
