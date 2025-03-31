
using System;

namespace Corral.Models
{
    public class TimeManagementModel
    {
        public int TMID { get; set; }                // Integer type for TMID
        public int ManagerID { get; set; }            // Integer type for ManagerID
        public string EmployeeName { get; set; }      // String for employee name (computed field)
        public int StoreID { get; set; }              // Integer type for StoreID
        public int DepartmentID { get; set; }         // Integer type for DepartmentID
        public decimal Rate { get; set; }             // Money type (decimal with 4 precision)
        public DateTime ServerTimeIn { get; set; }    // DateTime for ServerTimeIn
        public DateTime ServerTimeOut { get; set; }   // DateTime for ServerTimeOut
        public bool Approved { get; set; }            // Numeric (1/0), boolean-like (true/false)
        public int ApprovedBy { get; set; }           // Integer type for ApprovedBy (ManagerID)
        public string ApprovedByName { get; set; }    // String for ApprovedBy Name (computed field)
        public DateTime? BreakStartTime { get; set; } // Nullable DateTime for BreakStartTime
        public DateTime? BreakEndTime { get; set; }   // Nullable DateTime for BreakEndTime
    }
}
