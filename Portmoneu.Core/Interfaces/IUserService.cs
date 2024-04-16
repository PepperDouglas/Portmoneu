using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using Portmoneu.Models.Helpers;

namespace Portmoneu.Core.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<AdminRegisterDTO>> RegisterAdmin(AdminRegisterDTO adminRegisterDto);
        Task<ServiceResponse<Customer>> RegisterCustomer(CustomerRegisterDTO customerRegisterDTO);
        Task<ServiceResponse<string>> UserLogin(UserCredentials credentials);
        Task<ServiceResponse<Account>> AddNewAccount(NewAccountDTO newAccount, string customerId);
    }
}
