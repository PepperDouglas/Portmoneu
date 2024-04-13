using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Models.Profiles
{
    public class AccountOutProfile : Profile
    {
        public AccountOutProfile() {
            CreateMap<Account, AccountOutDTO>()
                .ForMember(des => des.Balance,
                option => option.MapFrom(src => src.Balance));
        }
    }
}
