using Microsoft.AspNetCore.Identity;
using Portmoneu.Models.Identity;
using System.Security.Claims;

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
        Task<bool> doesUserExist(string username);
    }
}
