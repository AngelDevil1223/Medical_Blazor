using BlazorEnterprise.Server.Data;
using BlazorEnterprise.Server.Models;
using BlazorEnterprise.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEnterprise.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        [Route("AllRoles"), HttpGet]
        public IEnumerable<RoleViewModel> AllRoles()
        {
            var users = _context.Roles.Select(x => new RoleViewModel()
            {
                RoleId = x.Id,
                RoleName = x.Name
            });
            return users;
        }

        [Route("Create"), HttpPost]
        public string Create([FromBody] RoleViewModel role)
        {
            try
            {
                var rl = _roleManager.FindByNameAsync(role.RoleName).Result;
                if (rl != null)
                    return "Duplicate role";
                _roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = role.RoleName,
                    NormalizedName = role.RoleName.ToLower()
                });
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [Route("Delete"), HttpPost]
        public string Delete([FromBody] string roleId)
        {
            try
            {
                var role = _context.Roles.Find(roleId);
                if (role != null)
                {
                    _context.UserRoles.Where(x => x.RoleId == roleId)
                        .ToList().ForEach(r => _context.Remove(r));
                    _context.Remove(role);
                    _context.SaveChanges();
                    return "Success";
                }
                return "User does not exist";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
