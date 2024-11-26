using AutoMapper;
using Neighbor.Contract.DTOs.MemberDTOs;
using Neighbor.Domain.Entities;
namespace Neighbor.Application.Mapper;

public class LessorProfile : Profile
{
    public LessorProfile()
    {
        CreateMap<Lessor, LessorDTO>();
    }
}
