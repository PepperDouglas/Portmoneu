using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Data.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _ciContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //next line should probably be in a different repo, if its not
        //ApplicationUser, but ApplicationRole or something
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepo(ApplicationDbContext ciContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager) {
            _ciContext = ciContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<bool> UserExists(string username) {
            return await _ciContext.Users.AnyAsync(u => u.UserName == username);
        }

        public async Task<IdentityResult> CreateUser(ApplicationUser user, string password) {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task CheckRoleExist(string rolename) {
            //not completely separated concerns
            var doesRoleExist = await _roleManager.RoleExistsAsync(rolename);
            if (!doesRoleExist) {
                await _roleManager.CreateAsync(new IdentityRole(rolename));
            }
        }

        public async Task<IdentityResult> AddRoleToUser(ApplicationUser user, string role) {
            return await _userManager.AddToRoleAsync(user, role);
        }
    }
}
