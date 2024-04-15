using Portmoneu.Core.Interfaces;
using Portmoneu.Core.Services;
using Portmoneu.Data.Interfaces;
using Portmoneu.Data.Repos;

namespace Portmoneu.Api.Extensions
{
    public static class TransientExtensions
    {
        public static IServiceCollection AddTransientExtended(this IServiceCollection services) {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepo, UserRepo>();
            services.AddTransient<ICustomerRepo, CustomerRepo>();
            services.AddTransient<IAccountRepo, AccountRepo>();
            services.AddTransient<IDispositionRepo, DispositionRepo>();
            services.AddTransient<ILoanRepo, LoanRepo>();
            services.AddTransient<ILoanService, LoanService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ITransactionRepo, TransactionRepo>();
            services.AddTransient<IAccountTypeRepo, AccountTypeRepo>();
            return services;
        }
    }
}
