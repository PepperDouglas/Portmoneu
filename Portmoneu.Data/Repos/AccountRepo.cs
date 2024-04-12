using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Data.Repos
{
    public class AccountRepo : IAccountRepo
    {
        private readonly BankAppData _bankAppData;

        public AccountRepo(BankAppData bankAppData) {
            _bankAppData = bankAppData;
        }

        public async Task CreateAccount(Account account) {
            await _bankAppData.Accounts.AddAsync(account);
            await _bankAppData.SaveChangesAsync();
        }
    }
}
