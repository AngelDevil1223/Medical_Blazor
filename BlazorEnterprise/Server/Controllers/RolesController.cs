using BlazorEnterprise.Server.Data;
using BlazorEnterprise.Server.Models;
using BlazorEnterprise.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorEnterprise.Server.Controllers
{
    [Route("[controller]")]
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
                var res = _roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = role.RoleName,
                    NormalizedName = role.RoleName.ToLower()
                }).Result;
                if (res.Succeeded)
                {
                    rl = _roleManager.FindByNameAsync(role.RoleName).Result;
                    foreach(var p in role.Permissions)
                    {
                        var pm = new RolePermission()
                        {
                            NavigationItem = p.NavigationObject,
                            Name = p.Name,
                            RoleId = rl.Id
                        };
                        _context.RolePermissions.Add(pm);
                    }
                    _context.SaveChanges();
                    return "Success";
                }
                else
                {
                    return string.Join(';', res.Errors.Select(x => x.Description));
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("Edit"), HttpPost]
        public string Edit([FromBody] RoleViewModel role)
        {
            try
            {
                var rl = _context.Roles.Include(c => c.Permissions).FirstOrDefault(x => x.Id == role.RoleId);
                foreach (var p in role.Permissions)
                {
                    if(p.Id<=0)
                    {
                        var pm = rl.Permissions.FirstOrDefault(x => x.Name == p.Name);
                        if (pm == null)
                        {
                            pm = new RolePermission()
                            {
                                NavigationItem = p.NavigationObject,
                                Name = p.Name,
                                RoleId = rl.Id
                            };
                            _context.RolePermissions.Add(pm);
                        }
                    }
                }
                foreach(var p in rl.Permissions.Where(x=>x.Id>0))
                {
                    var pm = role.Permissions.FirstOrDefault(x => x.Id == p.Id);
                    if(pm==null)
                    {
                        _context.RolePermissions.Remove(p);
                    }
                }
                _context.SaveChanges();
                return "Success";
            }
            catch (DbUpdateException ex)
            {
                return ex.InnerException.Message;
            }
        }
        [Route("RoleDetails"), HttpGet]
        public RoleViewModel RoleDetails(string roleId)
        {
            #pragma warning disable CS8603 // Possible null reference return.
            return _context.Roles.Include(c => c.Permissions).Where(x => x.Id == roleId)
                .Select(c => new RoleViewModel()
                {
                    RoleId = c.Id,
                    RoleName = c.Name,
                    Permissions = c.Permissions.Select(p => new RolePermissionModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        NavigationObject = p.NavigationItem
                    }).ToList()
                }).FirstOrDefault();
            #pragma warning restore CS8603 // Possible null reference return.
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
