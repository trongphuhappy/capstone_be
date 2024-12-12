using AutoMapper;
using Moq;
using Neighbor.Application.UseCases.V2.Queries.Surcharges;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Surcharges;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Neighbor.Contract.Services.Surcharges.Response;

namespace Neighbor.Application.UnitTests.UseCases.V2.Queries.Surcharges
{
    public class GetAllSurchargesQueryHandlerTests
    {
        private readonly Mock<IDPUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetAllSurchargesQueryHandler _handler;

        public GetAllSurchargesQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IDPUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetAllSurchargesQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WhenSurchargesAreFound_ReturnsSuccess()
        {
            // Arrange
            var query = new Query.GetAllSurchargesQuery
            (
                PageIndex: 1,
                PageSize: 10,
                FilterParams: null,
                SelectedColumns: null
            );
            var surcharge1Id = Guid.NewGuid();
            var surcharge2Id = Guid.NewGuid();
            var surcharges = new PagedResult<Surcharge>
            (
                items: new List<Surcharge>
                {
                    Surcharge.CreateSurchargeForTest(surcharge1Id, "Surcharge 1", "Surcharge 1 Description"),
                    Surcharge.CreateSurchargeForTest(surcharge2Id, "Surcharge 2", "Surcharge 2 Description"),
                },
                totalCount: 2,
                pageIndex: 1,
                pageSize: 10,
                totalPages: 1
            );

            var mappedResult = new PagedResult<SurchargeResponse>
            (
                items: new List<SurchargeResponse>
                {
                    new SurchargeResponse(surcharge1Id, "Surcharge 1", "Surcharge 1 Description"),
                    new SurchargeResponse(surcharge2Id, "Surcharge 2", "Surcharge 2 Description"),
                },
                totalCount: 2,
                pageIndex: 1,
                pageSize: 10,
                totalPages: 1
            );

            _mockUnitOfWork.Setup(u => u.SurchargeRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
                .ReturnsAsync(surcharges);

            _mockMapper.Setup(m => m.Map<PagedResult<SurchargeResponse>>(surcharges))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var successResponse = result as Result<Success<PagedResult<SurchargeResponse>>>;
            Assert.Equal(MessagesList.SurchargeGetAllSurchargesSuccess.GetMessage().Code, result.Value?.Code);
            Assert.Equal(MessagesList.SurchargeGetAllSurchargesSuccess.GetMessage().Message, result.Value?.Message);
            Assert.NotNull(result.Value?.Data);
            Assert.Equal(2, result.Value?.Data.Items.Count);

            _mockUnitOfWork.Verify(u => u.SurchargeRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
            _mockMapper.Verify(m => m.Map<PagedResult<SurchargeResponse>>(surcharges), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoSurchargesFound_ReturnsSurchargeNotFoundException()
        {
            // Arrange
            var query = new Query.GetAllSurchargesQuery
            (
                PageIndex: 1,
                PageSize: 10,
                FilterParams: null,
                SelectedColumns: null
            );

            var surcharges = new PagedResult<Surcharge>
            (
                items: new List<Surcharge>(),
                totalCount: 0,
                pageIndex: 1,
                pageSize: 10,
                totalPages: 0
            );

            var mappedResult = new PagedResult<SurchargeResponse>
            (
                items: new List<SurchargeResponse>(),
                totalCount: 0,
                pageIndex: 1,
                pageSize: 10,
                totalPages: 0
            );

            _mockUnitOfWork.Setup(u => u.SurchargeRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
                .ReturnsAsync(surcharges);

            _mockMapper.Setup(m => m.Map<PagedResult<SurchargeResponse>>(surcharges))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var successResponse = result as Result<Success<PagedResult<SurchargeResponse>>>;
            Assert.Equal(MessagesList.SurchargeNotFoundAnyException.GetMessage().Code, result.Value?.Code);
            Assert.Equal(MessagesList.SurchargeNotFoundAnyException.GetMessage().Message, result.Value?.Message);
            Assert.NotNull(result.Value?.Data);
            Assert.Empty(result.Value?.Data.Items);

            _mockUnitOfWork.Verify(u => u.SurchargeRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
            _mockMapper.Verify(m => m.Map<PagedResult<SurchargeResponse>>(surcharges), Times.Once);
        }
    }
}
