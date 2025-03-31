using System;
using System.Collections.Generic;
using System.Data;

namespace Corral.Models
{
    public class EditManagerViewModel
    {
        public int ManagerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool Active { get; set; }
        public int? HomeStore { get; set; }
        public int? GlobalStaffLinkStaffID { get; set; }
        public DateTime? AccountActiveDate { get; set; }
        public List<Module> AvailableModules { get; set; } = new List<Module>();

       
    }


    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

