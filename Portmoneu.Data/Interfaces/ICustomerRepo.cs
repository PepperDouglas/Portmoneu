using Portmoneu.Models.Entities;

namespace Portmoneu.Data.Interfaces
{
    public interface ICustomerRepo
    {
        Task<Customer> GetCustomer(int customerId);
        Task RegisterCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
    }
}
