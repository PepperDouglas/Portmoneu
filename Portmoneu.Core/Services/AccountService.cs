using AutoMapper;
using Portmoneu.Core.Interfaces;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using Portmoneu.Models.Helpers;

namespace Portmoneu.Core.Services
{
    public class AccountService : IAccountService {
        private readonly IAccountRepo _accountRepo;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly ITransactionRepo _transactionRepo;

        public AccountService(IAccountRepo accountRepo, IUserRepo userRepo, IMapper mapper, ITransactionRepo transactionRepo) {
            _accountRepo = accountRepo;
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
            
            List<AccountOutDTO> accountsOut = new List<AccountOutDTO>();
            accounts.ForEach(account => {
                if (account.AccountTypes == null) {
                    throw new Exception("Contains typeless accounts");
                }
                string accountType = account.AccountTypes.TypeName;
                AccountOutDTO acc = new AccountOutDTO(accountType, account.Balance);
                accountsOut.Add(acc);
            });
  
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
            
            var user = await _userRepo.GetUser(username);
            
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
            
            //start tracking, but does not save
            _accountRepo.AwaitUpdateAccount(actorAccount);
            _accountRepo.AwaitUpdateAccount(recieverAccount);
            
            var moneyOne = actorAccount.Balance;
            
            var transaction = _mapper.Map<Transaction>(transactionDto);
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
