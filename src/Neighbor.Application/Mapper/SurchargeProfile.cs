using AutoMapper;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Domain.Entities;
using static Neighbor.Contract.Services.Surcharges.Response;

namespace Neighbor.Application.Mapper;

public class SurchargeProfile : Profile
{
    public SurchargeProfile()
    {
        CreateMap<Surcharge, SurchargeResponse>();
        CreateMap<PagedResult<Surcharge>, PagedResult<SurchargeResponse>>();
    }
}
