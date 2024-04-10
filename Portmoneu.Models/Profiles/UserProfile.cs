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
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CustomerRegisterDTO, ApplicationUser>()
                .ForMember(des => des.UserName,
                option => option.MapFrom(src => src.Alias));
        }
    }
}
