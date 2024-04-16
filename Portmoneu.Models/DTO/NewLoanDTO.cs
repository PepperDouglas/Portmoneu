namespace Portmoneu.Models.DTO
{
    public record NewLoanDTO
    (
        int AccountID,
        decimal Amount,
        int Duration
    );
}
