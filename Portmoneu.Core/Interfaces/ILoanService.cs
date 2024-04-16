using Portmoneu.Models.DTO;
using Portmoneu.Models.Helpers;

namespace Portmoneu.Core.Interfaces
{
    public interface ILoanService
    {
        Task<ServiceResponse<NewLoanDTO>> CreateLoan(NewLoanDTO newLoan);
    }
}
