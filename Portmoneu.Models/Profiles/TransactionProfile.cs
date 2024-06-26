﻿using AutoMapper;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Entities;

namespace Portmoneu.Models.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile() {
            CreateMap<TransactionDTO, Transaction>()
                .ForMember(des => des.Amount,
                option => option.MapFrom(src => src.Amount))
                .ForMember(des => des.AccountId,
                option => option.MapFrom(src => src.SenderAccount))
                .ForMember(des => des.Account,
                option => option.MapFrom(src => src.RecieverAccount));
        }
    }
}
