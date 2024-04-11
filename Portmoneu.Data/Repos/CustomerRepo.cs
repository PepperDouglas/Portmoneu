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
    }
}
