using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEnterprise.Shared
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            UserId = "0";
            FullName = "";
            EmailAddress = "";
            PhoneNumber = "";
            Password = "";
            Role = "0";
            RoleId = "0";
        }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Role { get; set; }
        [Required(ErrorMessage = "A role is required")]
        public string RoleId { get; set; }
    }

    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
