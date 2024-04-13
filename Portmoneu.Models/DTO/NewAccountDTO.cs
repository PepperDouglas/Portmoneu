using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Models.DTO
{
    public record NewAccountDTO
    (
        string frequency,
        int accountTypeId
    );
}
