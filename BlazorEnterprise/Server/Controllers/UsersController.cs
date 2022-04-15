using BlazorEnterprise.Server.Data;
using BlazorEnterprise.Server.Models;
using BlazorEnterprise.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorEnterprise.Server.Controllers
{
    [Route("api/[controller]")]
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
            var users = _context.Users.Select(x => new UserViewModel()
            {
                EmailAddress = x.Email,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                UserId = x.Id
            });
            return users;
        }

        [Route("Create"), HttpPost]
        public string Create([FromBody] UserViewModel user)
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
                    FullName = user.FullName
                };
                _userManager.CreateAsync(usr, user.Password);
                return "Success";
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
                if (usr != null)
                {
                    _context.Attach(usr);
                    usr.FullName = user.FullName;
                    usr.PhoneNumber = user.PhoneNumber;
                    _context.Entry(usr).Property(p => p.FullName).IsModified = true;
                    _context.Entry(usr).Property(p => p.PhoneNumber).IsModified = true;
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

        [Route("AddRole"), HttpPost]
        public string AddRole([FromBody] UserRoleViewModel urole)
        {
            try
            {
                var rl = new IdentityUserRole<string>()
                {
                    RoleId = urole.RoleId,
                    UserId = urole.UserId
                };
                _context.UserRoles.Add(rl);
                _context.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [Route("RevokeRole"), HttpPost]
        public string RevokeRole([FromBody] UserRoleViewModel urole)
        {
            try
            {
                var rl = _context.UserRoles.FirstOrDefault(x => x.UserId == urole.UserId && x.RoleId == urole.RoleId);
                _context.Remove(rl);
                _context.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
