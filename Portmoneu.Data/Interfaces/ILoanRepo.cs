using Portmoneu.Models.Entities;

namespace Portmoneu.Data.Interfaces
{
    public interface ILoanRepo
    {
        Task RegisterLoan(Loan loan);
    }
}
