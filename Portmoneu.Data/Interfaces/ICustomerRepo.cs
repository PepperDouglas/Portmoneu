using Portmoneu.Models.Entities;
using Portmoneu.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Data.Interfaces
{
    public interface ICustomerRepo
    {
        Task<Customer> GetCustomer(int customerId);
        Task RegisterCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
    }
}
