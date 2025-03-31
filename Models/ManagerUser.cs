namespace Corral.Models;

public class ManagerUser
{
    public int ManagerID { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Active { get; set; } // "0" or "1" (Active/Inactive)
    public int? HomeStore { get; set; } // Store ID
    public int? SecurityLevel { get; set; } // Security clearance level
    public DateTime? AccountActiveDate { get; set; } // Account activation date
    public string FirstName { get; set; }
    public string MiddleName { get; set; } // Middle name added
    public string LastName { get; set; }

    public string Password { get; set; } // Consider masking or encrypting
    public int? GlobalStaffLinkStaffID { get; set; } // Linked staff ID (nullable)

    // Access permissions
    public bool AllowAdminScreen { get; set; } // Admin screen access
    public decimal AllowMultiStore { get; set; } // Multi-store access
    public decimal AllowIntranetAccess { get; set; } // Intranet access
    public bool AllowTypeLogin { get; set; } // TimeClock login access

    // Account & security details
    public DateTime? AccountExpireDate { get; set; } // Expiration date
    public DateTime? FingerEnrollmentDate { get; set; } // Fingerprint enrollment date
    public string TimeClockID { get; set; } // TimeClock system ID
    public string TimeClockPassCode { get; set; } // TimeClock login passcode

    // Additional fields (if required)
    public DateTime? LastLoginDate { get; set; } // Last login timestamp
    public bool AllowShiftApproval { get; set; } // Shift approval access
}
