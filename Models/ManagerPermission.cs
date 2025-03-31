namespace Corral.Models
{
    public class ManagerPermission
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescription { get; set; }
        public string FoundInModule { get; set; }
        public int ManagerID { get; set; }
    }
}
