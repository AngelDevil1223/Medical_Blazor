using BlazorEnterprise.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace BlazorEnterprise.Server.Data
{
    public class IdentityDataInitializer
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public IdentityDataInitializer(IServiceProvider serviceProvider)
        {
            userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        }
        public void SeedData()
        {
            SeedRoles();
            SeedUsers();
        }

        private void SeedUsers()
        {
            if (userManager.FindByNameAsync("demo@users.com").Result == null)
            {
                var user = new ApplicationUser();
                user.UserName = "demo@users.com";
                user.Email = "demo@users.com";
                user.FullName = "System Admin";
                user.EmailConfirmed = true;
                IdentityResult result = userManager.CreateAsync(user, "Abc.123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        private void SeedRoles()
        {
            if (!roleManager.RoleExistsAsync("User").Result)
            {
                var role = new ApplicationRole();
                role.Name = "User";
                var res = roleManager.CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new ApplicationRole();
                role.Name = "Admin";
                var res = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
