using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Identity;

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
