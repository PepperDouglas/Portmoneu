using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Models.DTO
{
    public record TransactionDTO
    (
        int SenderAccount,
        decimal Amount,
        int RecieverAccount
    );
}
