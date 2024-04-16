using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Identity;
using System.Security.Claims;

namespace Portmoneu.Data.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _ciContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
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

        public async Task<SignInResult> SignInTrial(string name, string password) {
            return await _signInManager.PasswordSignInAsync(name, password, false, false);
        }

        public async Task<ICollection<Claim>> GetClaims(ApplicationUser user) {
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<ICollection<string>> GetRoles(ApplicationUser user) {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<ApplicationUser> GetUser(string username) {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<bool> doesUserExist(string username) {
            var user = await _userManager.FindByNameAsync(username);
            return user != null;
        }
    }
}
