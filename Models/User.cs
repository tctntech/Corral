namespace Corral.Models
{
    public class User

    {
        public int ManagerID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Active { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int SecurityLevel { get; set; }
        public int? GlobalStaffLinkStaffID { get; set; }
        public bool AllowAdminScreen { get; set; }
        public decimal DefaultRate { get; set; }
        public string HourCalcOption { get; set; }
        public bool AllowMultiStore { get; set; }
        public bool AllowIntranetAccess { get; set; }
        
        public DateTime? AccountExpireDate { get; set; }
        public DateTime? FingerEnrollmentDate { get; set; }
        public bool AllowTypeLogin { get; set; }
        public string TimeClockID { get; set; }  // String type for TimeClockID

        public string TimeClockPassCode { get; set; }
        // Add additional properties as necessary
    }
}
