using BlazorEnterprise.Server.Data;
using BlazorEnterprise.Server.Models;
using BlazorEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorEnterprise.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Route("AllUsers"),HttpGet]
        public IEnumerable<UserViewModel> AllUsers()
        {
            var uulist = from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         select new UserViewModel()
                         {
                             EmailAddress = u.Email,
                             FullName = u.FullName,
                             PhoneNumber = u.PhoneNumber,
                             UserId = u.Id,
                             Role = r.Name,
                             RoleId = r.Id
                         };

            return uulist;
        }

        [Route("UserDetails"), HttpGet]
        public UserViewModel UserDetails(string id)
        {
            var uulist = from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where u.Id==id
                         select new UserViewModel()
                         {
                             EmailAddress = u.Email,
                             FullName = u.FullName,
                             PhoneNumber = u.PhoneNumber,
                             UserId = u.Id,
                             Role = r.Name,
                             RoleId = r.Id
                         };

            return uulist.FirstOrDefault();
        }

        [Route("HasPermission"), HttpGet]
        public bool HasPermission(string uid,string nav)
        {
            var uulist = from p in _context.RolePermissions
                         join r in _context.Roles on p.RoleId equals r.Id
                         join ur in _context.UserRoles on r.Id equals ur.RoleId
                         join u in _context.Users on ur.UserId equals u.Id
                         where (p.NavigationItem == nav && u.UserName == uid) || r.Name == "Admin"
                         select p.NavigationItem;

            return uulist.Count() > 0;
        }

        [Route("AddUser"), HttpPost]
        public string AddUser([FromBody] UserViewModel user)
        {
            try
            {
                var usr = _userManager.FindByNameAsync(user.EmailAddress).Result;
                if (usr != null)
                    return "User already exists";
                usr = new ApplicationUser()
                {
                    UserName = user.EmailAddress,
                    Email = user.EmailAddress,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    EmailConfirmed=true
                };
                var res=_userManager.CreateAsync(usr, user.Password).Result;
                if (res.Succeeded)
                {
                    _context.UserRoles.Add(new IdentityUserRole<string>()
                    {
                        RoleId = user.RoleId,
                        UserId = usr.Id
                    });
                    _context.SaveChanges();
                    return "Success";
                }
                return string.Join('\n', res.Errors.Select(x => x.Description));
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("Edit"), HttpPost]
        public string Edit([FromBody] UserViewModel user)
        {
            try
            {
                var usr = _context.Users.Find(user.UserId);
                var uroles = _context.UserRoles.Where(x => x.UserId == user.UserId).ToList();
                if (usr != null)
                {
                    _context.Attach(usr);
                    usr.FullName = user.FullName;
                    usr.PhoneNumber = user.PhoneNumber;
                    _context.Entry(usr).Property(p => p.FullName).IsModified = true;
                    _context.Entry(usr).Property(p => p.PhoneNumber).IsModified = true;
                    uroles.Where(x => x.RoleId != user.RoleId).ToList().ForEach(x => _context.Remove(x));
                    if (uroles.Where(x => x.RoleId == user.RoleId).Count() == 0)
                    {
                        _context.UserRoles.Add(new IdentityUserRole<string>()
                        {
                            RoleId = user.RoleId,
                            UserId = user.UserId
                        });
                    }
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

        [Route("Delete"), HttpPost]
        public string Delete([FromBody] string user)
        {
            try
            {
                var usr = _context.Users.Find(user);
                if (usr != null)
                {
                    _context.Remove(usr);
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
        [Route("UserPermissions"), HttpGet]
        public IEnumerable<RolePermissionModel> UserPermissions(string userName)
        {
#pragma warning disable CS8603 // Possible null reference return.
            var userId = _context.Users.FirstOrDefault(x => x.UserName == userName).Id;
            var roleId = _context.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
            var perms= _context.RolePermissions.Where(x => x.RoleId == roleId)
                .Select(c => new RolePermissionModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    NavigationObject = c.NavigationItem
                }).ToList();
            return perms;
            #pragma warning restore CS8603 // Possible null reference return.
        }
    }
}