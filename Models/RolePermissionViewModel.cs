namespace MyDemoApp.Models
{
    public class RolePermissionViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Module { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }
}
