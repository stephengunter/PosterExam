using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Helpers;

namespace ApplicationCore.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> FetchUsersAsync(string role = "", string keyword = "");
        IEnumerable<IdentityRole> FetchRoles();

        IEnumerable<IdentityRole> GetRolesByUserId(string userId);
    }

    public class UsersService : IUsersService
    {
        DefaultContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersService(DefaultContext context, UserManager<User> userManager, RoleManager<IdentityRole>  roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<User>> FetchUsersAsync(string role = "", string keyword = "")
        {
            var users = _userManager.Users;

            if (!String.IsNullOrEmpty(role))
            {
                var selectedRole = await _roleManager.FindByNameAsync(role);
                if (selectedRole != null)
                {
                    var userIdsInRole = _context.UserRoles.Where(x => x.RoleId == selectedRole.Id).Select(b => b.UserId).Distinct().ToList();
                    users = users.Where(user => userIdsInRole.Contains(user.Id));
                }
            }
            

            if (String.IsNullOrEmpty(keyword)) return users;
            if (users.IsNullOrEmpty()) return users;

            return users.Where(u => u.UserName.CaseInsensitiveContains(keyword) || u.Name.CaseInsensitiveContains(keyword));

        }

        public IEnumerable<IdentityRole> FetchRoles() => _roleManager.Roles.ToList();

        public IEnumerable<IdentityRole> GetRolesByUserId(string userId)
        {
            var userRoles = _context.UserRoles.Where(x => x.UserId == userId);
            var roleIds = userRoles.Select(ur => ur.RoleId);

            return _roleManager.Roles.Where(r => roleIds.Contains(r.Id));
        }
       
    }
}
