using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Models.Helpers
{
    public record UserCredentials(
        string Username,
        string Password
    );
}
