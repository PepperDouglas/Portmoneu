using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;
using Portmoneu.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Models.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile() {
            CreateMap<NewAccountDTO, Account>()
                .ForMember(des => des.Frequency,
                option => option.MapFrom(src => src.frequency))
                .ForMember(des => des.AccountTypesId,
                option => option.MapFrom(src => src.accountTypeId));
        }
    }
}
