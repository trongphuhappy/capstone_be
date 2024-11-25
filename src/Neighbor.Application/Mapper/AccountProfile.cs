using AutoMapper;
using Neighbor.Contract.DTOs.MemberDTOs;
using Neighbor.Domain.Entities;

namespace Neighbor.Application.Mapper;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Account, ProfileDTO>();
    }
}
