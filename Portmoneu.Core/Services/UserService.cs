using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Portmoneu.Core.Interfaces;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using Portmoneu.Models.Helpers;
using Portmoneu.Models.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Portmoneu.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly ICustomerRepo _customerRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly IDispositionRepo _dispositionRepo;
        private readonly IAccountTypeRepo _accountTypeRepo;

        public UserService(IUserRepo userRepo, IMapper mapper, ICustomerRepo customerRepo, IAccountRepo accountRepo, IDispositionRepo dispositionRepo, IAccountTypeRepo accountTypeRepo) {
            _userRepo = userRepo;
            _mapper = mapper;
            _customerRepo = customerRepo;
            _accountRepo = accountRepo;
            _dispositionRepo = dispositionRepo;
            _accountTypeRepo = accountTypeRepo;
        }

        public async Task<ServiceResponse<AdminRegisterDTO>> RegisterAdmin(AdminRegisterDTO adminRegisterDTO) {
            
            var doesUserExist = await _userRepo.UserExists(adminRegisterDTO.Name);
            
            if (doesUserExist) {
                return new ServiceResponse<AdminRegisterDTO>
                {
                    Message = "Username already exists"
                };
            }
            
            var userModel = _mapper.Map<ApplicationUser>(adminRegisterDTO);
            
            var createUserResult = await _userRepo.CreateUser(userModel, adminRegisterDTO.Password);
            
            if (!createUserResult.Succeeded) {
                var errorlist = createUserResult.Errors.ToList();
                string errormessage = "";
                foreach (var item in errorlist)
                {
                    errormessage += " " + item.Description;
                }
                return new ServiceResponse<AdminRegisterDTO>()
                {
                    Message = "Could not create the user: " + errormessage
                };
            }

            await _userRepo.CheckRoleExist("Admin");

            var addRoleToUserResult = await _userRepo.AddRoleToUser(userModel, "Admin");

            if (!addRoleToUserResult.Succeeded) {
                return new ServiceResponse<AdminRegisterDTO>()
                {
                    Message = "Could not attach admin role to new user"
                };
            }

            return new ServiceResponse<AdminRegisterDTO>()
            {
                Message = "New Admin User added",
                Success = true,
                Data = adminRegisterDTO
            };
        }


        public async Task<ServiceResponse<string>> UserLogin(UserCredentials credentials) {
            
            var result = await _userRepo.SignInTrial(credentials.Username, credentials.Password);
            if (result.Succeeded) {
                //add JWT here depending on role              
                var user = await _userRepo.GetUser(credentials.Username);
                var userClaims = await _userRepo.GetClaims(user);
                var userRoles = await _userRepo.GetRoles(user);
                var roleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role));
                
                var claims = new List<Claim>(userClaims);
                claims.AddRange(roleClaims);
                
                if (userRoles.Contains("User")) {
                    claims.Add(new Claim("CustomerId", user.CustomerId.ToString()));
                } else if (userRoles.Contains("Admin")) {
                    string rolePurpose = "AdminRoleLogin";
                    Console.WriteLine(rolePurpose);
                }
                var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretKey12345!#123456789101112"));
                var signInCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5271/",
                    audience: "http://localhost:5271/",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: signInCredentials

                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return new ServiceResponse<string>()
                {
                    Message = "User logged in",
                    Success = true,
                    Data = tokenString
                };

            } else {
                return new ServiceResponse<string>()
                {
                    Message = "Wrong username or password",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<Customer>> RegisterCustomer(CustomerRegisterDTO customerRegisterDTO) {
            bool userAliasExists = await _userRepo.UserExists(customerRegisterDTO.Alias);
            if (userAliasExists) {
                throw new Exception("User alias already exists");
            }

            var customer = _mapper.Map<Customer>(customerRegisterDTO);
            await _customerRepo.RegisterCustomer(customer);
     
            var appUser = _mapper.Map<ApplicationUser>(customerRegisterDTO);
            appUser.CustomerId = customer.CustomerId;
            await _userRepo.CreateUser(appUser, customerRegisterDTO.Password);
      
            await _userRepo.CheckRoleExist("User");

            var addRoleToUserResult = await _userRepo.AddRoleToUser(appUser, "User");

            if (!addRoleToUserResult.Succeeded) {
                return new ServiceResponse<Customer>()
                {
                    Message = "Could not attach 'User' role to new user"
                };
            }

            //1. create a bank account and relate it to a customer by saving details
            //inside the disposition table OR
            //2. create a bank account and add it to the (nav prop) of the customers
            //dispositions;
            //Method 1:
            DateOnly createdDate = DateOnly.FromDateTime(DateTime.Now);
            int accountTypesId = 1;
            string frequency = "Monthly";
            decimal balance = 0;
            var account = new Account()
            {
                Frequency = frequency,
                Balance = balance,
                Created = createdDate,
                AccountTypesId = accountTypesId,
            };

            await _accountRepo.CreateAccount(account);

            var disposition = new Disposition()
            {
                AccountId = account.AccountId,
                CustomerId = customer.CustomerId,
                Type = "OWNER"
            };

            await _dispositionRepo.RecordDispositionRelation(disposition);

            return new ServiceResponse<Customer>()
            {
                Message = "New 'User' user added",
                Success = true,
                Data = customer
            };
        }

        public async Task<ServiceResponse<Account>> AddNewAccount(NewAccountDTO newAccount, string customerId) {

            var user = await _userRepo.GetUser(customerId);
            if (user == null) {
                throw new Exception("No such customer (token error)");
            }

            int customerID = (int)user.CustomerId;

            //Here we can check if the user is allowed to create such an account
            var validAccountTypes = await _accountTypeRepo.GetAllAccountTypes();
            if (!validAccountTypes.Any(acc => acc.AccountTypeId == newAccount.accountTypeId)) {
                throw new Exception("Not a valid account type");
            }
            
            //lägg till kontot, få id
            var account = _mapper.Map<Account>(newAccount);
            account.Created = DateOnly.FromDateTime(DateTime.Now);
            account.Balance = 0;
            await _accountRepo.CreateAccount(account);

            //hämta customern
            var customer = await _customerRepo.GetCustomer(customerID);

            //skapa upp en disposition
            var disposition = new Disposition()
            {
                Type = "OWNER",
                CustomerId = customerID,
                AccountId = account.AccountId
            };

            //lägg till disposition på customer (metod 2)
            customer.Dispositions.Add(disposition);

            //spara kunden igen
            await _customerRepo.UpdateCustomer(customer);

            return new ServiceResponse<Account>()
            {
                Message = "New account added",
                Success = true,
                Data = account
            };
        }
    }
}
