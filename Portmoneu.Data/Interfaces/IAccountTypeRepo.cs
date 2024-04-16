using Portmoneu.Models.Entities;

namespace Portmoneu.Data.Interfaces
{
    public interface IAccountTypeRepo
    {
        Task<List<AccountType>> GetAllAccountTypes();
    }
}
