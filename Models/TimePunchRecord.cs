
namespace Corral.Models
{


    public class TimePunchRecord
    {
        public int TMID { get; set; }
        public string EmployeeName { get; set; }
        public int StoreID { get; set; }
        public decimal Rate { get; set; }
        public DateTime ServerTimeIn { get; set; }
        public DateTime? ServerTimeOut { get; set; }
        public decimal TotalHours { get; set; }
        public string ApprovedByName { get; set; }
    }

}
