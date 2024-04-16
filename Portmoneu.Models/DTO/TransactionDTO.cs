namespace Portmoneu.Models.DTO
{
    public record TransactionDTO
    (
        int SenderAccount,
        decimal Amount,
        int RecieverAccount
    );
}
