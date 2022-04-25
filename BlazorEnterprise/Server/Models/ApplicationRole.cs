using Microsoft.AspNetCore.Identity;

namespace BlazorEnterprise.Server.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() => Permissions = new HashSet<RolePermission>();
        public virtual ICollection<RolePermission> Permissions { get; set; }
    }

    public class RolePermission
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public virtual ApplicationRole Role { get; set; }
        public string NavigationItem { get; set; }
        public string Name { get; set; }
    }
}
