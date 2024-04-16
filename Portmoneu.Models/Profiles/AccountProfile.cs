using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;

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
