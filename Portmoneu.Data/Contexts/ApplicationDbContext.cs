using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portmoneu.Models.Identity;

namespace Portmoneu.Data.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder builder) { base.OnModelCreating(builder); }
    }
}
