using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using Portmoneu.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Core.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResponse<TransactionDTO>> CreateTransaction(TransactionDTO transactionDto, string username);
        Task<ServiceResponse<List<Transaction>>> GetTransactionDetails(int accountid, string customername);
        Task<ServiceResponse<List<AccountOutDTO>>> RetrieveAccounts(string customer);
    }
}
