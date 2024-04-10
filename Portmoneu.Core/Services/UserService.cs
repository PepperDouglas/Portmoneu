using AutoMapper;
using Portmoneu.Core.Interfaces;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Helpers;
using Portmoneu.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepo userRepo, IMapper mapper) {
            _userRepo = userRepo;
            _mapper = mapper;
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
    }
}
