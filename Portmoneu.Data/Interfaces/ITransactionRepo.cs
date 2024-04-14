using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Portmoneu.Data.Interfaces
{
    public interface ITransactionRepo
    {
        Task CreateTransaction(Models.Entities.Transaction transaction);
        Task<List<Models.Entities.Transaction>> RetrieveTransactionsForAccount(int accountid);
    }
}
