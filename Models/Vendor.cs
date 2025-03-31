namespace Corral.Models
{
    public class Vendor
{
    public int VendorID { get; set; }
    public string VendorName { get; set; }
    public string ContactName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string Terms { get; set; }
    public bool? CalculateDueDateWithTerms { get; set; }
    public string OurAccountNumber { get; set; }
    public string Comments { get; set; }

    // Active is stored as nchar, so we handle it as a string and convert it later
   
    public bool? Active { get; set; }  // Change from string to bool?


    public string Website { get; set; }
    public string Email { get; set; }
    public string ProductsProvided { get; set; }
    public decimal? GlobalVendor { get; set; }
    public DateTime? DateAdded { get; set; }
    public int? AddedBy { get; set; }
    public int? MaxDaysToAllowFuturePost { get; set; }

    // Exclusion Details
    public int? ExclusionID { get; set; }
    public string ExclusionStoreID { get; set; } // Changed from int? to string
    public string ExclusionStoreCity { get; set; }

    // Inclusion Details
    public int? InclusionID { get; set; }
    public string InclusionStoreID { get; set; } // Changed from int? to string
    public string InclusionStoreCity { get; set; }
    public List<StoreVendorExclusions> StoreVendorExclusions { get; set; }
    public List<StoreVendorInclusion> StoreVendorInclusions { get; set; }
    }
}
