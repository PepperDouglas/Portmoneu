using Microsoft.EntityFrameworkCore;
using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Entities;

namespace Portmoneu.Data.Repos
{
    public class AccountTypeRepo : IAccountTypeRepo
    {
        private readonly BankAppData _bankAppData;

        public AccountTypeRepo(BankAppData bankAppData) {
            _bankAppData = bankAppData;
        }

        public async Task<List<AccountType>> GetAllAccountTypes() {
            return await _bankAppData.AccountTypes.Select(acctype => new AccountType
            {
                AccountTypeId = acctype.AccountTypeId,
                TypeName = acctype.TypeName
            }).ToListAsync();
        }
    }
}
