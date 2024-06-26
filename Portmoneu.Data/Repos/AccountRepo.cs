﻿using Microsoft.EntityFrameworkCore;
using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Entities;

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

        public async Task<Account> RetrieveAccount(int accountId) {
            return await _bankAppData.Accounts.FindAsync(accountId);
        }

        public async Task UpdateAccount(Account account) {
            _bankAppData.Accounts.Update(account);
            await _bankAppData.SaveChangesAsync();
        }

        public void AwaitUpdateAccount(Account account) {
            _bankAppData.Accounts.Update(account);
        }

        public async Task<List<Account>> RetrieveAccounts(int customerId) {
            return await _bankAppData.Dispositions
                .Where(d => d.CustomerId == customerId)
                .Include(d => d.Account)
                    .ThenInclude(a => a.AccountTypes)
                .Select(d => d.Account)
                .ToListAsync();
        }
    }
}
