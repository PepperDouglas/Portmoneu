using AutoMapper;
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

        public UserService(IUserRepo userRepo, IMapper mapper, ICustomerRepo customerRepo) {
            _userRepo = userRepo;
            _mapper = mapper;
            _customerRepo = customerRepo;
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
                var claims = new List<Claim> { }
                .Concat(userClaims).Concat(roleClaims); //creates claims from claims and roles
                //claims.Add(new Claim(ClaimTypes.Role, ))
                //om någon av Rolesen är user, så måste ett customerID komma med
                if (userRoles.Contains("User")) {
                    //add customerID to token here
                    claims = claims.Append(new Claim("CustomerId", user.CustomerId.ToString())).ToList();
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

            return new ServiceResponse<Customer>()
            {
                Message = "New 'User' user added",
                Success = true,
                Data = customer
            };
        }

    }
}
