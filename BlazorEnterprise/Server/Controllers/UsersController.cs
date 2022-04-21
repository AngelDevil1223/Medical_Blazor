﻿using BlazorEnterprise.Server.Data;
using BlazorEnterprise.Server.Models;
using BlazorEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<BlazorEnterprise.Shared.UserViewModel> AllUsers()
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
        public BlazorEnterprise.Shared.UserViewModel UserDetails(string id)
        {
            var uulist = from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where u.Id==id
                         select new BlazorEnterprise.Shared.UserViewModel()
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

        [Route("AddUser"), HttpPost]
        public string AddUser([FromBody] BlazorEnterprise.Shared.UserViewModel user)
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
                //_context.UserRoles.Add(new IdentityUserRole<string>()
                //{
                //    RoleId = user.RoleId,
                //    UserId = usr.Id
                //});
                //_context.SaveChanges();
                return "Success";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("Edit"), HttpPost]
        public string Edit([FromBody] BlazorEnterprise.Shared.UserViewModel user)
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
    }
}