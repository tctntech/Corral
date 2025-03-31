using System;
using System.ComponentModel.DataAnnotations;

namespace Corral.Models
{
    public class Manager
    {
        [Key]
        public int ManagerID { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public int? SecurityLevel { get; set; }

        public int? HomeStore { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DefaultRate { get; set; }

        public DateTime? AccountActiveDate { get; set; }
        public DateTime? AccountExpireDate { get; set; }

        public int? GlobalStaffLinkStaffID { get; set; }

        public bool EnableTimeClockLogin { get; set; }
        public bool EnableInventoryLogin { get; set; }
        public bool CanApproveOwnShifts { get; set; }
        public bool Is2FAEnabled { get; set; }

        public string HashedPassword { get; set; }
        public string TotpSecret { get; set; }

        // Converts Active from varchar("Yes"/"No") to bool
        public string ActiveString { get; set; }
        public bool Active => ActiveString?.ToLower() == "yes";

    }
}
