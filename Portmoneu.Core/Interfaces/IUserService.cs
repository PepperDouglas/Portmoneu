﻿using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using Portmoneu.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Core.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<AdminRegisterDTO>> RegisterAdmin(AdminRegisterDTO adminRegisterDto);
        Task<ServiceResponse<Customer>> RegisterCustomer(CustomerRegisterDTO customerRegisterDTO);
        Task<ServiceResponse<string>> UserLogin(UserCredentials credentials);
    }
}
