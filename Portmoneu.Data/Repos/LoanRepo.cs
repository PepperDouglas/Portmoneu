using Portmoneu.Data.Contexts;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.Entities;

namespace Portmoneu.Data.Repos
{
    public class LoanRepo : ILoanRepo
    {
        private readonly BankAppData _bankAppData;

        public LoanRepo(BankAppData bankAppData) {
            _bankAppData = bankAppData;
        }

        public async Task RegisterLoan(Loan loan) {
            await _bankAppData.Loans.AddAsync(loan);
            await _bankAppData.SaveChangesAsync();
        }
    }
}
