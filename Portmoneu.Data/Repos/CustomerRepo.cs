using Microsoft.EntityFrameworkCore;
using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Data.Repos
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly BankAppData _bankAppData;

        public CustomerRepo(BankAppData bankAppData) {
            _bankAppData = bankAppData;
        }

        public async Task RegisterCustomer(Customer customer) {
            await _bankAppData.AddAsync(customer);
            await _bankAppData.SaveChangesAsync();
        }

        public async Task<Customer> GetCustomer(int customerId) {
            return await _bankAppData.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task UpdateCustomer(Customer customer) {
            _bankAppData.Update(customer);
            await _bankAppData.SaveChangesAsync();
        }
    }
}
