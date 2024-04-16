using Portmoneu.Models.Entities;

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
