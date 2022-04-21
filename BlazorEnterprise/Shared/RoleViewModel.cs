using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEnterprise.Shared
{
    public class RoleViewModel
    {
        public RoleViewModel() => Permissions = new List<RolePermissionModel>();
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RolePermissionModel> Permissions { get; set; }
    }

    public class RolePermissionModel
    {
        public int Id { get; set; }
        public string NavigationObject { get; set; }
        public string Name { get; set; }
    }
}
