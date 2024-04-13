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
    public class LoanService : ILoanService
    {
        private readonly ILoanRepo _loanRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly IMapper _mapper;

        public LoanService(ILoanRepo loanRepo, IMapper mapper, IAccountRepo accountRepo) {
            _loanRepo = loanRepo;
            _mapper = mapper;
            _accountRepo = accountRepo;
        }

        public async Task<ServiceResponse<NewLoanDTO>> CreateLoan(NewLoanDTO newLoan) {
            var account = await _accountRepo.RetrieveAccount(newLoan.AccountID);
            if (account == null) {
                return new ServiceResponse<NewLoanDTO> {
                    Success = false,
                    Message = "Account does not exist",
                    Data = newLoan
                };
            }
            //map the loan
            var loan = _mapper.Map<Loan>(newLoan);
            loan.Status = "Running";
            loan.Date = DateOnly.FromDateTime(DateTime.Now);
            loan.Payments = newLoan.Amount / newLoan.Duration;

            await _loanRepo.RegisterLoan(loan);

            //update use account
            account.Balance += newLoan.Amount;

            await _accountRepo.UpdateAccount(account);

            return new ServiceResponse<NewLoanDTO>
            {
                Success = true,
                Message = "Account balance updated",
                Data = newLoan
            };
        }
    }
}
