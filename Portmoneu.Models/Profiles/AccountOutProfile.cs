using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;

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
