using AutoMapper;
using Neighbor.Domain.Entities;
using Neighbor.Contract.Services.Categories;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Categories.Response;

namespace PawFund.Application.Mapper;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponse>();
        CreateMap<PagedResult<Category>, PagedResult<CategoryResponse>>();
    }
}
