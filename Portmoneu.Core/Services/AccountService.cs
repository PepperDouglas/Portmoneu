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
    public class AccountService : IAccountService {
        private readonly IAccountRepo _accountRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly ITransactionRepo _transactionRepo;

        public AccountService(IAccountRepo accountRepo, ICustomerRepo customerRepo, IUserRepo userRepo, IMapper mapper, ITransactionRepo transactionRepo) {
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _transactionRepo = transactionRepo;
        }

        public async Task<ServiceResponse<List<AccountOutDTO>>> RetrieveAccounts(string customer) {
            var user = await _userRepo.GetUser(customer) ?? throw new Exception("Invalid customer validation");
            var accounts = await _accountRepo.RetrieveAccounts((int)user.CustomerId);
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

        public async Task<ServiceResponse<List<Transaction>>> GetTransactionDetails(int accountid, string customername) {
            var user = await _userRepo.GetUser(customername);
            var userAccounts = await _accountRepo.RetrieveAccounts((int)user.CustomerId);
            if (userAccounts.Count == 0) {
                throw new Exception("No available accounts");
            }
            var relevantAccount = userAccounts.FirstOrDefault(acc => acc.AccountId == accountid);
            if (relevantAccount == null) {
                throw new Exception("Not account of user");
            }
            var transactionDetails = await _transactionRepo.RetrieveTransactionsForAccount(accountid);
            return new ServiceResponse<List<Transaction>>
            {
                Success = true,
                Message = "",
                Data = transactionDetails
            };
        }
        

        public async Task<ServiceResponse<TransactionDTO>> CreateTransaction(TransactionDTO transactionDto, string username) {
            if (transactionDto.SenderAccount == transactionDto.RecieverAccount) {
                throw new Exception("Cannot handle transaction");
            }
            //get customer from user
            var user = await _userRepo.GetUser(username);
            //check if is own account (get all accounts, see if one of them matches the accountID)
            var userAccounts = await _accountRepo.RetrieveAccounts((int)user.CustomerId);
            if (userAccounts.Count == 0) {
                return new ServiceResponse<TransactionDTO>
                {
                    Success = false,
                    Message = "No accounts available"
                };
            }
            var actorAccount = userAccounts.FirstOrDefault(acc => acc.AccountId == transactionDto.SenderAccount);
            var recieverAccount = await _accountRepo.RetrieveAccount(transactionDto.RecieverAccount);
            string isFoundMessage = recieverAccount == null ? "Reciever could not be found" : "Your account could not be found";
            if (actorAccount == null || recieverAccount == null) {
                return new ServiceResponse<TransactionDTO>
                {
                    Success = false,
                    Message = isFoundMessage
                };
            }
            //get relevant account
            //var actorAccount = userAccounts.FirstOrDefault(a => a.AccountId == transactionDto.SenderAccount);
            //remove money from that account
            if (actorAccount.Balance >= transactionDto.Amount) {
                actorAccount.Balance -= transactionDto.Amount;
                recieverAccount.Balance += transactionDto.Amount;
            } else {
                return new ServiceResponse<TransactionDTO>
                {
                    Success = false,
                    Message = "Not enough credit on this account"
                };
            }
            //these changes only updates but doesnt save the account
            //save it
            _accountRepo.AwaitUpdateAccount(actorAccount);
            //get the account being sent to
            //update it
            //save it
            _accountRepo.AwaitUpdateAccount(recieverAccount);
            //REMAINING MONEYT
            var moneyOne = actorAccount.Balance;
            //create a map of the Transaction
            var transaction = _mapper.Map<Transaction>(transactionDto);
            //add manual things
            transaction.Date = DateOnly.FromDateTime(DateTime.Now);
            transaction.Operation = "Transfer";
            transaction.Type = "Debit";
            transaction.Amount = transaction.Amount * (-1); 
            transaction.Balance = moneyOne;
            
            //save the transaction
            await _transactionRepo.CreateTransaction(transaction);
            return new ServiceResponse<TransactionDTO>
            {
                Success = true,
                Message = "Transaction successful",
                Data = transactionDto
            };
        }
    }
}
