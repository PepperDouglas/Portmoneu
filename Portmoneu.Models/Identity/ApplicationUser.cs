using Microsoft.AspNetCore.Identity;

namespace Portmoneu.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        //Det kommer att skapas ett antal default properties. Men här kan du lägga
        //till egna properties
        public int? CustomerId { get; set; }
    }
}
