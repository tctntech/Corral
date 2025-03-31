

namespace Corral.Models
{
    public class TimeClockEntry
{
    public int TMID { get; set; }
    public int ManagerID { get; set; }
    public string EmployeeName { get; set; }
    public int StoreID { get; set; }
    public decimal Rate { get; set; }
    public DateTime ServerTimeIn { get; set; }
    public DateTime? ServerTimeOut { get; set; }
    public bool Approved { get; set; }
    public int? ApprovedBy { get; set; }
    public string ApprovedByName { get; set; }
    public decimal TimeWorked => ServerTimeOut.HasValue
      ? (decimal)(ServerTimeOut.Value - ServerTimeIn).TotalHours
      : 0; // Auto-calculated

    }
}