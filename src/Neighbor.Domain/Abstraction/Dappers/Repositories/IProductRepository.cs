﻿using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Domain.Entities;
using static Neighbor.Contract.Services.Products.Filter;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IProductRepository : IGenericRepository<Domain.Entities.Product>
{
    Task<PagedResult<Product>> GetPagedAsync(int pageIndex, int pageSize, ProductFilter filterParams, string[] selectedColumns);
    Task<Product> GetDetailsAsync(Guid productId);
    Task<PagedResult<Product>> GetProductsInWishlistAsync(Guid accountId, int pageIndex, int pageSize, ProductWishlistFilter filterParams, string[] selectedColumns);
}
