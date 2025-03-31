namespace Corral.Models
{
    public class TimePunchRequest
    {
        public int ManagerID { get; set; }
        public int StoreID { get; set; }
        public decimal Rate { get; set; }
        public DateTime TimeIn { get; set; }
    }
}
