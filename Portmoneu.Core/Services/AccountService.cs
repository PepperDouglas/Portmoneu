using AutoMapper;
using Portmoneu.Core.Interfaces;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using Portmoneu.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepo accountRepo, ICustomerRepo customerRepo, IUserRepo userRepo, IMapper mapper) {
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<AccountOutDTO>>> RetrieveAccounts(string customer) {
            var user = await _userRepo.GetUser(customer) ?? throw new Exception("Invalid customer validation");
            var accounts =  await _accountRepo.RetrieveAccounts((int)user.CustomerId);
            if (accounts.Count == 0) {
                return new ServiceResponse<List<AccountOutDTO>> { 
                    Success = false,
                    Message = "No accounts found"
                };
            }
            //var accountsOut = _mapper.Map<List<AccountOutDTO>>(accounts);
            List<AccountOutDTO> accountsOut = new List<AccountOutDTO>();
            accounts.ForEach(account => {
                if (account.AccountTypes == null) {
                    throw new Exception("Contains typeless accounts");
                }
                string accountType = account.AccountTypes.TypeName;
                AccountOutDTO acc = new AccountOutDTO(accountType, account.Balance);
                accountsOut.Add(acc);
            });
            //get all types of accounts
            //for loop where they are bound
            //instead of 2TWO requests, we now included the types in the first req
            

            return new ServiceResponse<List<AccountOutDTO>>()
            {
                Success = true,
                Message = "",
                Data = accountsOut
            };

        }
    }
}
