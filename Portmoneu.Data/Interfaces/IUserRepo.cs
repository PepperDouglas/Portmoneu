using Microsoft.AspNetCore.Identity;
using Portmoneu.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Data.Interfaces
{
    public interface IUserRepo
    {
        Task<bool> UserExists(string username);

        Task<IdentityResult> CreateUser(ApplicationUser user, string password);

        Task CheckRoleExist(string rolename);

        Task<IdentityResult> AddRoleToUser(ApplicationUser user, string role);
        Task<SignInResult> SignInTrial(string name, string password);
        Task<ICollection<string>> GetRoles(ApplicationUser user);
        Task<ICollection<Claim>> GetClaims(ApplicationUser user);
        Task<ApplicationUser> GetUser(string username);
    }
}
