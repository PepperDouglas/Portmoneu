using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Models.Profiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile() {
            CreateMap<AdminRegisterDTO, ApplicationUser>()
                .ForMember(des => des.UserName,
                option => option.MapFrom(src => src.Name));
        }
        
    }
}
