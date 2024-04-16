using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;

namespace Portmoneu.Models.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile() {
            CreateMap<CustomerRegisterDTO, Customer>()
                .ForMember(des => des.Gender,
                option => option.MapFrom(src => src.Gender))
                .ForMember(des => des.Givenname,
                option => option.MapFrom(src => src.Givenname))
                .ForMember(des => des.Surname,
                option => option.MapFrom(src => src.Surname))
                .ForMember(des => des.Streetaddress,
                option => option.MapFrom(src => src.Streetaddress))
                .ForMember(des => des.City,
                option => option.MapFrom(src => src.City))
                .ForMember(des => des.Zipcode,
                option => option.MapFrom(src => src.Zipcode))
                .ForMember(des => des.Country,
                option => option.MapFrom(src => src.Country))
                .ForMember(des => des.Country,
                option => option.MapFrom(src => src.Country));
        }
    }
}
