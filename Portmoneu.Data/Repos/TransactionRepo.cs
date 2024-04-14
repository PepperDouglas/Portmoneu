using Microsoft.EntityFrameworkCore;
using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Portmoneu.Data.Repos
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly BankAppData _bankAppData;

        public TransactionRepo(BankAppData bankAppData) {
            _bankAppData = bankAppData;
        }

        public async Task CreateTransaction(Models.Entities.Transaction transaction) {
            await _bankAppData.Transactions.AddAsync(transaction);
            await _bankAppData.SaveChangesAsync();
        }
    }
}
