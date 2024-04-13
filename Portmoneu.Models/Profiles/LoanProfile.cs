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
    public class LoanProfile : Profile
    {
        public LoanProfile() {
            CreateMap<NewLoanDTO, Loan>()
                .ForMember(des => des.Duration,
                option => option.MapFrom(src => src.Duration))
                .ForMember(des => des.AccountId,
                option => option.MapFrom(src => src.AccountID))
                .ForMember(des => des.Amount,
                option => option.MapFrom(src => src.Amount));


        }
    }
}
