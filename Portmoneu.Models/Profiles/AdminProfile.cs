using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Identity;

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
