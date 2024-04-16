using Portmoneu.Models.Entities;

namespace Portmoneu.Data.Interfaces
{
    public interface ITransactionRepo
    {
        //Task CreateTransaction(Models.Entities.Transaction transaction);
        Task CreateTransaction(Transaction transaction, Transaction recTransaction);
        Task<List<Models.Entities.Transaction>> RetrieveTransactionsForAccount(int accountid);
    }
}
