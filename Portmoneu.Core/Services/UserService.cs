using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Portmoneu.Core.Interfaces;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using Portmoneu.Models.Helpers;
using Portmoneu.Models.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            //does user exist?
            var doesUserExist = await _userRepo.UserExists(adminRegisterDTO.Name);
            //if yes, return the "flag"
            if (doesUserExist) {
                return new ServiceResponse<AdminRegisterDTO>
                {
                    Message = "Username already exists"
                };
            }
            //create a user with mapping profile
            var userModel = _mapper.Map<ApplicationUser>(adminRegisterDTO);
            //create user and return the IdentityResult
            var createUserResult = await _userRepo.CreateUser(userModel, adminRegisterDTO.Password);
            var errorlist = createUserResult.Errors.ToList();
            if (!createUserResult.Succeeded) {
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
            //get result of signing in trial
            var result = await _userRepo.SignInTrial(credentials.Username, credentials.Password);
            if (result.Succeeded) {
                //add JWT here depending on role
                //await _userRepo.CheckRoleExist("User"); //this is for creating user
                var user = await _userRepo.GetUser(credentials.Username);//get the user
                var userClaims = await _userRepo.GetClaims(user);
                var userRoles = await _userRepo.GetRoles(user);
                var roleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role));
                //var claims = new List<Claim> { }
                //.Concat(userClaims).Concat(roleClaims); //creates claims from claims and roles
                var claims = new List<Claim>(userClaims);
                claims.AddRange(roleClaims);
                //claims.Add(new Claim(ClaimTypes.Role, ))
                //om någon av Rolesen är user, så måste ett customerID komma med
                if (userRoles.Contains("User")) {
                    //add customerID to token here
                    //int id = (int)user.CustomerId;
                    claims.Add(new Claim("CustomerId", user.CustomerId.ToString()));
                    //claims = claims.Append(new Claim("CustomerId", user.CustomerId.ToString())).ToList();
                } else if (userRoles.Contains("Admin")) {
                    string testpurpose = "role cna be checked";
                    Console.WriteLine(testpurpose);
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
            //Here we need to check that the alias does not exist already, very important
            bool userAliasExists = await _userRepo.UserExists(customerRegisterDTO.Alias);
            if (userAliasExists) {
                throw new Exception("User alias already exists");
            }


            //vi måste mappa till customer och registrera
            var customer = _mapper.Map<Customer>(customerRegisterDTO);
            await _customerRepo.RegisterCustomer(customer); //nu ska den ha id
            
            //mappa en AppUser med name
            var appUser = _mapper.Map<ApplicationUser>(customerRegisterDTO);

            //ta ut id från svaret vi får tillbaka efter save
            //manuellt addera customerID
            appUser.CustomerId = customer.CustomerId;


            //spara ned med lösenord
            await _userRepo.CreateUser(appUser, customerRegisterDTO.Password);
            
            //see that User role exists, otherwise create it
            await _userRepo.CheckRoleExist("User");

            //addera customer Role : User
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

            //we try to save it down
            await _accountRepo.CreateAccount(account);

            //here is the difference, we save the Disp manually
            //instead of attaching to and re-saving the customer
            //we might need to also attach an account and customer
            //directly, not only the ID's
            //but in that case, we might as well use method 2
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
            //get the customer for that name 
            var user = await _userRepo.GetUser(customerId);

            
            //konvertera string till int
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
