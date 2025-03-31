using System.Collections.Generic;
using System.Data.Odbc;
using Microsoft.AspNetCore.Mvc;
using Corral.Models;

public class LocationInfoController : Controller
{
    private readonly string connectionString = "Driver={ODBC Driver 17 for SQL Server};Server=192.168.5.15;Database=TexasCorralQA;Uid=zpatel;Pwd=@TCzp2219!;"; 

    public ActionResult Index()
    {
        List<StoreLocation> locations = new List<StoreLocation>();

        using (OdbcConnection conn = new OdbcConnection(connectionString))
        {
            conn.Open();
            string query =@"SELECT StoreNumber, StoreName, Address, City, State, ZipCode, PhoneNumber, 
                        FaxNumber, AmexFee, Corporate, TaxProfile, Active, MerchantID, ClientID, TerminalID, 
                        VisaMCMerchant, AmexMerchant, DiscoverMerchant, CCUserName, CCPassword, CCRegistrationNumber, 
                        CCUnlockCode, DDHLName, TipAllocation, MinimumWage, PayrollID, ManageMenu, BackBankValue, 
                        BarBankValue, VendBankValue, EndOfLunch, FederalTaxID, LegalName, DDVersion, HasUsablePriorYearData, 
                        StoreIDMenuTemplate, CompanyType, ConceptID, OpenDate, CloseDate, HLSerialNumber, LocalDBConnectionString
                        FROM TexasCorralQA.General.StoreInformation";


            using (OdbcCommand cmd = new OdbcCommand(query, conn))
            using (OdbcDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    locations.Add(new StoreLocation
                    {
                        StoreNumber = reader["StoreNumber"].ToString(),
                        StoreName = reader["StoreName"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        ZipCode = reader["ZipCode"].ToString(),
                        PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "N/A",

                        FaxNumber = reader["FaxNumber"].ToString(),
                        AmexFee = reader["AmexFee"].ToString(),
                        Corporate = reader["Corporate"].ToString(),
                        TaxProfile = reader["TaxProfile"].ToString(),
                        Active = reader["Active"] != DBNull.Value && int.TryParse(reader["Active"].ToString(), out int activeValue) && activeValue == 1,
                        VisaMCMerchant = reader["VisaMCMerchant"] != DBNull.Value && int.TryParse(reader["VisaMCMerchant"].ToString(), out int visaValue) && visaValue == 1,
                        AmexMerchant = reader["AmexMerchant"] != DBNull.Value && int.TryParse(reader["AmexMerchant"].ToString(), out int amexValue) && amexValue == 1,
                        DiscoverMerchant = reader["DiscoverMerchant"] != DBNull.Value && int.TryParse(reader["DiscoverMerchant"].ToString(), out int discoverValue) && discoverValue == 1,
                        HasUsablePriorYearData = reader["HasUsablePriorYearData"] != DBNull.Value && int.TryParse(reader["HasUsablePriorYearData"].ToString(), out int priorYearDataValue) && priorYearDataValue == 1,

                       
                        MerchantID = reader["MerchantID"].ToString(),
                        ClientID = reader["ClientID"].ToString(),
                        TerminalID = reader["TerminalID"].ToString(),                        
                        CCPassword = reader["CCPassword"].ToString(),
                        CCRegistrationNumber = reader["CCRegistrationNumber"].ToString(),
                        CCUserName = reader["CCUserName"].ToString(),
                        CCUnlockCode = reader["CCUnlockCode"].ToString(),
                        DDHLName = reader["DDHLName"].ToString(),
                        TipAllocation = reader["TipAllocation"].ToString(),
                        MinimumWage = reader["MinimumWage"].ToString(),
                        PayrollID = reader["PayrollID"].ToString(),
                        ManageMenu = reader["ManageMenu"].ToString(),
                        BackBankValue = reader["BackBankValue"].ToString(),
                        BarBankValue = reader["BarBankValue"].ToString(),
                        VendBankValue = reader["VendBankValue"].ToString(),
                        EndOfLunch = reader["EndOfLunch"].ToString(),
                        FederalTaxID = reader["FederalTaxID"].ToString(),
                        LegalName = reader["LegalName"].ToString(),
                        DDVersion = reader["DDVersion"].ToString(),StoreIDMenuTemplate = reader["StoreIDMenuTemplate"].ToString(),
                        CompanyType = reader["CompanyType"].ToString(),
                        ConceptID = reader["ConceptID"].ToString(),
                        OpenDate = reader["OpenDate"] != DBNull.Value ? (DateTime)reader["OpenDate"] : DateTime.MinValue, // Handle DBNull for DateTime
                        CloseDate = reader["CloseDate"] != DBNull.Value ? (DateTime)reader["CloseDate"] : DateTime.MinValue, // Handle DBNull for DateTime
                        HLSerialNumber = reader["HLSerialNumber"].ToString(),
                        LocalDBConnectionString = reader["LocalDBConnectionString"].ToString()

                    });

                }
            }
        }

        return View(locations);
    }
}
