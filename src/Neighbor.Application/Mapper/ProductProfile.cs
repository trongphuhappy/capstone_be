using AutoMapper;
using Neighbor.Domain.Entities;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Products.Response;

namespace Neighbor.Application.Mapper;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponse>();
        CreateMap<PagedResult<Product>, PagedResult<ProductResponse>>();
    }
}
