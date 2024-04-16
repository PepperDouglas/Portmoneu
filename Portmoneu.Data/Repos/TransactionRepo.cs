using Microsoft.EntityFrameworkCore;
using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;

namespace Portmoneu.Data.Repos
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly BankAppData _bankAppData;

        public TransactionRepo(BankAppData bankAppData) {
            _bankAppData = bankAppData;
        }

        public async Task CreateTransaction(Models.Entities.Transaction transaction, Models.Entities.Transaction recTransaction) {
            await _bankAppData.Transactions.AddAsync(transaction);
            await _bankAppData.Transactions.AddAsync(recTransaction);
            await _bankAppData.SaveChangesAsync();
        }

        public async Task<List<Models.Entities.Transaction>> RetrieveTransactionsForAccount(int accountid) {
            return await _bankAppData.Transactions.Where(trans => trans.AccountId == accountid)
                .Select(trans => new Models.Entities.Transaction
                {
                    TransactionId = trans.TransactionId,
                    AccountId = trans.AccountId,
                    Date = trans.Date,
                    Type = trans.Type,
                    Operation = trans.Operation,
                    Amount = trans.Amount,
                    Balance = trans.Balance,
                    Account = trans.Account
                })
                .ToListAsync();
        }
    }
}
