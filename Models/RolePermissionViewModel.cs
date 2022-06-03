using System.ComponentModel.DataAnnotations;

namespace MyDemoApp.Models
{
    public class RolePermissionViewModel
    {
        public int RoleId { get; set; }
        [Required(ErrorMessage ="RoleName is equired")]
        [RegularExpression(@"^[a-zA-Z\s]*$")]
        public string RoleName { get; set; }
        public List<ModulePermission> ModulePermissionList { get; set; }
    }
    public class ModulePermission
    {
        public int ModuleId { get; set; }
        public string Module { get; set; }
        public bool IsSelected { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }
}
