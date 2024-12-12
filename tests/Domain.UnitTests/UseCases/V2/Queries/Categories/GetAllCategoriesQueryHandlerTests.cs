using AutoMapper;
using Moq;
using Neighbor.Application.UseCases.V2.Queries.Categories;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Categories;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static Neighbor.Contract.Services.Categories.Response;

namespace Neighbor.Application.UnitTests.UseCases.V2.Queries.Categories;

public class GetAllCategoriesQueryHandlerTests
{
    private readonly Mock<IDPUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetAllSurchargesQueryHandler _handler;

    public GetAllCategoriesQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IDPUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetAllSurchargesQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoriesAreFound_ReturnsSuccess()
    {
        // Arrange
        var query = new Query.GetAllCategoriesQuery
        (
            PageIndex: 1,
            PageSize: 10,
            FilterParams: null,
            SelectedColumns: null
        );


        var categories = new PagedResult<Category>
        (
            items: new List<Category>
            {
                Category.CreateCategoryForTest(1, "Category 1", false, null),
                Category.CreateCategoryForTest(2, "Category 2", false, null),
            },
            totalCount: 2,
            pageIndex: 1,
            pageSize: 10,
            totalPages: 1
        );

        var mappedResult = new PagedResult<Response.CategoryResponse>
        (
            items: new List<Response.CategoryResponse>
            {
                new Response.CategoryResponse(1, "Category 1", false, null),
                new Response.CategoryResponse(2, "Category 2", false, null),
            },
            totalCount: 2,
            pageIndex: 1,
            pageSize: 10,
            totalPages: 1
        );

        _mockUnitOfWork.Setup(u => u.CategoryRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
            .ReturnsAsync(categories);

        _mockMapper.Setup(m => m.Map<PagedResult<CategoryResponse>>(categories))
            .Returns(mappedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        var successResponse = result as Result<Success<PagedResult<CategoryResponse>>>;
        Assert.Equal(MessagesList.CategoryGetAllCategoriesSuccess.GetMessage().Code, result.Value?.Code);
        Assert.Equal(MessagesList.CategoryGetAllCategoriesSuccess.GetMessage().Message, result.Value?.Message);
        Assert.NotNull(result.Value?.Data);
        Assert.Equal(2, result.Value?.Data.Items.Count);

        _mockUnitOfWork.Verify(u => u.CategoryRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
        _mockMapper.Verify(m => m.Map<PagedResult<CategoryResponse>>(categories), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoCategoriesFound_ReturnsCategoryNotFoundException()
    {
        // Arrange
        var query = new Query.GetAllCategoriesQuery
        (
            PageIndex: 1,
            PageSize: 10,
            FilterParams: null,
            SelectedColumns: null
        );

        var categories = new PagedResult<Category>
        (
            items: new List<Category>(),
            totalCount: 0,
            pageIndex: 1,
            pageSize: 10,
            totalPages: 0
        );

        var mappedResult = new PagedResult<CategoryResponse>
        (
            items: new List<CategoryResponse>(),
            totalCount: 0,
            pageIndex: 1,
            pageSize: 10,
            totalPages: 0
        );

        _mockUnitOfWork.Setup(u => u.CategoryRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
            .ReturnsAsync(categories);

        _mockMapper.Setup(m => m.Map<PagedResult<Response.CategoryResponse>>(categories))
            .Returns(mappedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        var successResponse = result as Result<Success<PagedResult<CategoryResponse>>>;
        Assert.Equal(MessagesList.CategoryNotFoundAnyException.GetMessage().Code, result.Value?.Code);
        Assert.Equal(MessagesList.CategoryNotFoundAnyException.GetMessage().Message, result.Value?.Message);
        Assert.NotNull(result.Value?.Data);
        Assert.Empty(result.Value?.Data.Items);

        _mockUnitOfWork.Verify(u => u.CategoryRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
        _mockMapper.Verify(m => m.Map<PagedResult<CategoryResponse>>(categories), Times.Once);
    }
}
