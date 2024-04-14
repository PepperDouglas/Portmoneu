using Portmoneu.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Data.Interfaces
{
    public interface IAccountRepo
    {
        void AwaitUpdateAccount(Account account);
        Task CreateAccount(Account account);
        Task<Account> RetrieveAccount(int accountId);
        Task<List<Account>> RetrieveAccounts(int customerId);
        Task UpdateAccount(Account account);
    }
}
