namespace Portmoneu.Data.Interfaces
{
    public interface ITransactionRepo
    {
        Task CreateTransaction(Models.Entities.Transaction transaction);
        Task<List<Models.Entities.Transaction>> RetrieveTransactionsForAccount(int accountid);
    }
}
