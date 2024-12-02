using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Categories;
using Neighbor.Domain.Abstraction.Dappers;

namespace Neighbor.Application.UseCases.V2.Queries.Categories;

public sealed class GetAllSurchargesQueryHandler : IQueryHandler<Query.GetAllCategoriesQuery, Success<PagedResult<Response.CategoryResponse>>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetAllSurchargesQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }
    public async Task<Result<Success<PagedResult<Response.CategoryResponse>>>> Handle(Query.GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        //Find all Categories
        var listCategories = await _dpUnitOfWork.CategoryRepositories.GetPagedAsync(request.PageIndex, request.PageSize, request.FilterParams, request.SelectedColumns);
        //Mapping Category to CategoryResponse
        var result = _mapper.Map<PagedResult<Response.CategoryResponse>>(listCategories);
        //Check if ListCategory is empty
        if (listCategories.Items.Count == 0)
        {
            return Result.Success(new Success<PagedResult<Response.CategoryResponse>>(MessagesList.CategoryNotFoundAnyException.GetMessage().Code, MessagesList.CategoryNotFoundAnyException.GetMessage().Message, result));

        }
        //Return result
        return Result.Success(new Success<PagedResult<Response.CategoryResponse>>(MessagesList.CategoryGetAllCategoriesSuccess.GetMessage().Code, MessagesList.CategoryGetAllCategoriesSuccess.GetMessage().Message, result));
    }
}
