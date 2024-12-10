using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Domain.Entities;
using System.Text;

namespace Neighbor.Infrastructure.Dapper.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly IConfiguration _configuration;
    public ProductRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<int> AddAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Product>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Product>? GetByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public async Task<Product> GetDetailsAsync(Guid productId)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();

            // Query the product
            var products = await connection.QueryAsync<Product, Lessor, Category, Product>(
                @"SELECT p.Id, p.Name, p.StatusType, p.Policies, p.Description, p.RejectReason, p.Rating, p.Price, p.Value, p.MaximumRentDays, p.ConfirmStatus, p.LessorId, p.CreatedDate, p.ModifiedDate AS ProductModifiedDate, l.Id as LessorId, l.WareHouseAddress, l.ShopName, l.AccountId, c.Id as CategoryId, c.Name, c.IsVehicle
              FROM Products p
              JOIN Lessors l ON l.Id = p.LessorId
              JOIN Categories c ON c.Id = p.CategoryId
              WHERE p.Id = @Id",
                (p, l, c) =>
                {
                    p.UpdateLessorProduct(l);
                    p.UpdateCategory(c);
                    return p;
                },
                new { Id = productId },
                splitOn: "ProductModifiedDate, LessorId, CategoryId"
            );

            if (products == null) return null;
            var product = products.ToList()[0];
            // Query related images
            var productImages = await connection.QueryAsync<Images>(
                @"SELECT ImageLink, ImageId, CreatedDate FROM Images WHERE ProductId = @Id", new { Id = productId });

            product.UpdateImagesProduct(productImages.ToList());

            // Query surcharges and related data
            var productSurcharges = await connection.QueryAsync<ProductSurcharge, Surcharge, ProductSurcharge>(
                @"SELECT ps.Id, ps.Price, ps.CreatedDate, ps.SurchargeId, ps.CreatedDate as ProductSurchargeCreatedDate,
                  s.Id, s.Name, s.Description, s.IsDeleted 
                FROM ProductSurcharges ps
                JOIN Surcharges s ON ps.SurchargeId = s.Id
                WHERE ps.ProductId = @Id",
                (ps, s) =>
                {
                    ps.UpdateSurcharge(s);
                    return ps;
                },
                new { Id = productId },
                splitOn: "ProductSurchargeCreatedDate");

            product.UpdateProductSurcharges(productSurcharges.ToList());

            // Query insurances and their images
            var insuranceData = new Dictionary<Guid, Insurance>();

            await connection.QueryAsync<Insurance, Images, Insurance>(
                @"SELECT i.Id, i.Name, i.IssueDate, i.ExpirationDate, i.CreatedDate AS InsuranceCreatedDate, 
                ii.ImageLink, ii.ImageId, ii.CreatedDate
                FROM Insurances i
                LEFT JOIN Images ii ON i.Id = ii.InsuranceId
                WHERE i.ProductId = @Id",
                (insurance, insuranceImage) =>
                {
                    if (!insuranceData.TryGetValue(insurance.Id, out var existingInsurance))
                    {
                        // If this insurance is not yet added, create it
                        existingInsurance = insurance;
                        existingInsurance.UpdateImagesInsurance(new List<Images>());
                        insuranceData.Add(existingInsurance.Id, existingInsurance);
                    }

                    // Add images to the insurance object
                    if (insuranceImage != null && !existingInsurance.Images.Any(img => img.ImageId == insuranceImage.ImageId))
                    {
                        existingInsurance.Images.Add(insuranceImage);
                    }

                    return existingInsurance;
                },
                new { Id = productId },
                splitOn: "InsuranceCreatedDate");

            // Update the product's insurance list
            product.UpdateInsurance(insuranceData.Values.ToList());

            return product;

        }
    }

    public async Task<PagedResult<Product>> GetPagedAsync(
    int pageIndex,
    int pageSize,
    Filter.ProductFilter filterParams,
    string[] selectedColumns)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            // Valid columns for selecting
            var validColumns = new HashSet<string>
        {
            "p.Id", "p.Name", "p.StatusType", "p.Policies", "p.Description", "p.RejectReason",
            "p.Rating", "p.Price", "p.Value", "p.MaximumRentDays", "p.ConfirmStatus", "p.LessorId", "p.CreatedDate",
            "p.ModifiedDate AS ProductModifiedDate", "i.ImageLink", "i.ImageId", "i.CreatedDate AS ImageCreatedDate", "l.Id", "l.WareHouseAddress", "l.ShopName", "l.AccountId", "l.CreatedDate AS LessorCreatedDate", "w.Id", "w.AccountId", "w.ModifiedDate AS WishlistModifiedDate", "c.Id", "c.Name", "c.IsVehicle"
        };

            var columns = selectedColumns?.Where(c => validColumns.Contains(c)).ToArray();

            // If no selected columns, select all
            var selectedColumnsString = columns?.Length > 0
                ? string.Join(", ", columns)
                : string.Join(", ", validColumns);

            // Base query for fetching data
            var queryBuilder = new StringBuilder($@"
            SELECT {selectedColumnsString} 
            FROM Products p
            LEFT JOIN Images i ON p.Id = i.ProductId
            LEFT JOIN Wishlists w on p.Id = w.ProductId
            JOIN Lessors l ON l.Id = p.LessorId
            JOIN Categories c ON c.Id = p.CategoryId
            WHERE 1=1");

            // Base query for total count
            var totalCountQuery = new StringBuilder($@"
            SELECT COUNT(DISTINCT p.Id) 
            FROM Products p
            LEFT JOIN Images i ON p.Id = i.ProductId
            LEFT JOIN Wishlists w on p.Id = w.ProductId
            JOIN Lessors l ON l.Id = p.LessorId
            JOIN Categories c ON c.Id = p.CategoryId
            WHERE 1=1");

            var parameters = new DynamicParameters();

            // Pagination and defaults
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize > 100 ? 100 : pageSize;

            // Apply filters
            if (filterParams?.Id.HasValue == true)
            {
                queryBuilder.Append(" AND p.Id = @Id");
                totalCountQuery.Append(" AND p.Id = @Id");
                parameters.Add("Id", filterParams.Id);
            }

            if (!string.IsNullOrEmpty(filterParams?.Name))
            {
                queryBuilder.Append(" AND p.Name LIKE @Name");
                totalCountQuery.Append(" AND p.Name LIKE @Name");
                parameters.Add("Name", $"%{filterParams.Name}%");
            }

            if (filterParams?.StatusType != null)
            {
                queryBuilder.Append(" AND p.StatusType = @StatusType");
                totalCountQuery.Append(" AND p.StatusType = @StatusType");
                parameters.Add("StatusType", filterParams.StatusType);
            }

            if (!string.IsNullOrEmpty(filterParams?.Policies))
            {
                queryBuilder.Append(" AND p.Policies LIKE @Policies");
                totalCountQuery.Append(" AND p.Policies LIKE @Policies");
                parameters.Add("Policies", $"%{filterParams.Policies}%");
            }

            if (!string.IsNullOrEmpty(filterParams?.Description))
            {
                queryBuilder.Append(" AND p.Description LIKE @Description");
                totalCountQuery.Append(" AND p.Description LIKE @Description");
                parameters.Add("Description", $"%{filterParams.Description}%");
            }

            if (filterParams?.Rating.HasValue == true)
            {
                queryBuilder.Append(" AND p.Rating = @Rating");
                totalCountQuery.Append(" AND p.Rating = @Rating");
                parameters.Add("Rating", filterParams.Rating);
            }

            if (filterParams?.Price.HasValue == true)
            {
                queryBuilder.Append(" AND p.Price = @Price");
                totalCountQuery.Append(" AND p.Price = @Price");
                parameters.Add("Price", filterParams.Price);
            }

            if (filterParams?.Value.HasValue == true)
            {
                queryBuilder.Append(" AND p.Value = @Value");
                totalCountQuery.Append(" AND p.Value = @Value");
                parameters.Add("Value", filterParams.Value);
            }

            if (filterParams?.MaximumRentDays.HasValue == true)
            {
                queryBuilder.Append(" AND p.MaximumRentDays = @MaximumRentDays");
                totalCountQuery.Append(" AND p.MaximumRentDays = @MaximumRentDays");
                parameters.Add("MaximumRentDays", filterParams.MaximumRentDays);
            }

            if (filterParams?.ConfirmStatus != null)
            {
                queryBuilder.Append(" AND p.ConfirmStatus = @ConfirmStatus");
                totalCountQuery.Append(" AND p.ConfirmStatus = @ConfirmStatus");
                parameters.Add("ConfirmStatus", filterParams.ConfirmStatus);
            }

            if (filterParams?.AccountLessorId.HasValue == true)
            {
                queryBuilder.Append(" AND l.AccountId = @AccountId");
                totalCountQuery.Append(" AND l.AccountId = @AccountId");
                parameters.Add("AccountId", filterParams.AccountLessorId);
            }

            if (filterParams?.CategoryId.HasValue == true)
            {
                queryBuilder.Append(" AND p.CategoryId = @CategoryId");
                totalCountQuery.Append(" AND p.CategoryId = @CategoryId");
                parameters.Add("CategoryId", filterParams.CategoryId);
            }
            if (filterParams?.IsVehicle.HasValue == true)
            {
                queryBuilder.Append(" AND c.IsVehicle = @IsVehicle");
                totalCountQuery.Append(" AND c.IsVehicle = @IsVehicle");
                parameters.Add("IsVehicle", filterParams.IsVehicle);
            }

            // Count TotalCount
            var totalCount = await connection.ExecuteScalarAsync<int>(totalCountQuery.ToString(), parameters);

            // Calculate TotalPages
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var offset = (pageIndex - 1) * pageSize;
            //Check if IsSortCreatedDayASC exist then Sort by CreatedDate
            if (filterParams?.IsSortCreatedDateASC.HasValue == true)
            {
                //Check if IsSortCreatedDayASC == true then Sort Created Date ASC
                if (filterParams?.IsSortCreatedDateASC == true)
                {
                    queryBuilder.Append($" ORDER BY p.CreatedDate OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
                }
                else
                {
                    queryBuilder.Append($" ORDER BY p.CreatedDate DESC OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
                }
            }
            else
            {
                queryBuilder.Append($" ORDER BY p.Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
            }

            // Dictionary for mapping products and their images
            var productDictionary = new Dictionary<Guid, Product>();

            // Execute the query
            var items = await connection.QueryAsync<Product, Images, Lessor, Wishlist, Category, Product>(
                queryBuilder.ToString(),
                (product, image, lessor, wishlist, category) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var existingProduct))
                    {
                        existingProduct = product;
                        existingProduct.UpdateImagesProduct(new List<Images>());
                        existingProduct.UpdateWishlistsProduct(new List<Wishlist>());
                        productDictionary.Add(product.Id, existingProduct);
                    }

                    // Add image if not already added
                    if (image != null && !existingProduct.Images.Any(img => img.ImageId == image.ImageId))
                    {
                        existingProduct.Images.Add(image);
                    }

                    // Add wishlist if not already added
                    if (wishlist != null && !existingProduct.Wishlists.Any(w => w.Id == wishlist.Id))
                    {
                        //Check if Account is User and Account has already added this Product to Wishlist or not
                        if (filterParams.AccountUserId != null)
                        {
                            if (wishlist.AccountId == filterParams.AccountUserId)
                            {
                                existingProduct.Wishlists.Add(wishlist);
                            }
                        }
                    }

                    existingProduct.UpdateLessorProduct(lessor);
                    existingProduct.UpdateCategory(category);
                    return existingProduct;
                },
                parameters,
                splitOn: "ProductModifiedDate, ImageCreatedDate, LessorCreatedDate, WishlistModifiedDate"
            );

            // Return paginated result
            return new PagedResult<Product>(
                productDictionary.Values.ToList(),
                pageIndex,
                pageSize,
                totalCount,
                totalPages
            );
        }
    }

    public async Task<PagedResult<Product>> GetProductsInWishlistAsync(Guid accountId,
    int pageIndex,
    int pageSize,
    Filter.ProductWishlistFilter filterParams,
    string[] selectedColumns)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            // Valid columns for selecting
            var validColumns = new HashSet<string>
        {
            "p.Id", "p.Name", "p.StatusType", "p.Policies", "p.Description", "p.RejectReason",
            "p.Rating", "p.Price", "p.Value", "p.MaximumRentDays", "p.ConfirmStatus", "p.LessorId", "p.CreatedDate",
            "p.ModifiedDate AS ProductModifiedDate", "i.ImageLink", "i.ImageId", "i.CreatedDate AS ImageCreatedDate", "l.Id", "l.WareHouseAddress", "l.ShopName", "l.CreatedDate AS LessorCreatedDate", "w.Id", "w.CreatedDate", "w.ModifiedDate AS WishlistModifiedDate", "c.Id", "c.Name", "c.IsVehicle"
        };

            var columns = selectedColumns?.Where(c => validColumns.Contains(c)).ToArray();

            // If no selected columns, select all
            var selectedColumnsString = columns?.Length > 0
                ? string.Join(", ", columns)
                : string.Join(", ", validColumns);

            // Base query for fetching data
            var queryBuilder = new StringBuilder($@"
            SELECT {selectedColumnsString} 
            FROM Products p
            LEFT JOIN Images i ON p.Id = i.ProductId
            JOIN Lessors l ON l.Id = p.LessorId
            JOIN Wishlists w ON p.Id = w.ProductId
            JOIN Categories c ON c.Id = p.CategoryId
            WHERE 1=1 AND w.AccountId = @AccountId");

            // Base query for total count
            var totalCountQuery = new StringBuilder($@"
            SELECT COUNT(DISTINCT p.Id) 
            FROM Products p
            LEFT JOIN Images i ON p.Id = i.ProductId
            JOIN Lessors l ON l.Id = p.LessorId
            JOIN Wishlists w ON p.Id = w.ProductId
            JOIN Categories c ON c.Id = p.CategoryId
            WHERE 1=1 AND w.AccountId = @AccountId");

            var parameters = new DynamicParameters();

            // Pagination and defaults
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize > 100 ? 100 : pageSize;
            parameters.Add("AccountId", accountId);
            // Apply filters
            if (filterParams?.Id.HasValue == true)
            {
                queryBuilder.Append(" AND p.Id = @Id");
                totalCountQuery.Append(" AND p.Id = @Id");
                parameters.Add("Id", filterParams.Id);
            }

            if (!string.IsNullOrEmpty(filterParams?.Name))
            {
                queryBuilder.Append(" AND p.Name LIKE @Name");
                totalCountQuery.Append(" AND p.Name LIKE @Name");
                parameters.Add("Name", $"%{filterParams.Name}%");
            }

            if (filterParams?.StatusType != null)
            {
                queryBuilder.Append(" AND p.StatusType = @StatusType");
                totalCountQuery.Append(" AND p.StatusType = @StatusType");
                parameters.Add("StatusType", filterParams.StatusType);
            }

            if (!string.IsNullOrEmpty(filterParams?.Policies))
            {
                queryBuilder.Append(" AND p.Policies LIKE @Policies");
                totalCountQuery.Append(" AND p.Policies LIKE @Policies");
                parameters.Add("Policies", $"%{filterParams.Policies}%");
            }

            if (!string.IsNullOrEmpty(filterParams?.Description))
            {
                queryBuilder.Append(" AND p.Description LIKE @Description");
                totalCountQuery.Append(" AND p.Description LIKE @Description");
                parameters.Add("Description", $"%{filterParams.Description}%");
            }

            if (filterParams?.Rating.HasValue == true)
            {
                queryBuilder.Append(" AND p.Rating = @Rating");
                totalCountQuery.Append(" AND p.Rating = @Rating");
                parameters.Add("Rating", filterParams.Rating);
            }

            if (filterParams?.Price.HasValue == true)
            {
                queryBuilder.Append(" AND p.Price = @Price");
                totalCountQuery.Append(" AND p.Price = @Price");
                parameters.Add("Price", filterParams.Price);
            }

            if (filterParams?.Value.HasValue == true)
            {
                queryBuilder.Append(" AND p.Value = @Value");
                totalCountQuery.Append(" AND p.Value = @Value");
                parameters.Add("Value", filterParams.Value);
            }

            if (filterParams?.MaximumRentDays.HasValue == true)
            {
                queryBuilder.Append(" AND p.MaximumRentDays = @MaximumRentDays");
                totalCountQuery.Append(" AND p.MaximumRentDays = @MaximumRentDays");
                parameters.Add("MaximumRentDays", filterParams.MaximumRentDays);
            }
            if (filterParams?.CategoryId.HasValue == true)
            {
                queryBuilder.Append(" AND p.CategoryId = @CategoryId");
                totalCountQuery.Append(" AND p.CategoryId = @CategoryId");
                parameters.Add("CategoryId", filterParams.CategoryId);
            }

            // Get total count and pages
            var totalCount = await connection.ExecuteScalarAsync<int>(totalCountQuery.ToString(), parameters);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Pagination logic
            var offset = (pageIndex - 1) * pageSize;
            //Check if IsSortCreatedDayASC exist then Sort by CreatedDate
            if (filterParams?.IsSortAddToWishlistDateASC.HasValue == true)
            {
                //Check if IsSortCreatedDayASC == true then Sort Created Date ASC
                if (filterParams?.IsSortAddToWishlistDateASC == true)
                {
                    queryBuilder.Append($" ORDER BY w.CreatedDate OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
                }
                else
                {
                    queryBuilder.Append($" ORDER BY w.CreatedDate DESC OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
                }
            }
            else
            {
                queryBuilder.Append($" ORDER BY p.Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
            }
            // Dictionary for mapping products and their images
            var productDictionary = new Dictionary<Guid, Product>();

            // Execute the query
            var items = await connection.QueryAsync<Product, Images, Lessor, Wishlist, Category, Product>(
                queryBuilder.ToString(),
                (product, image, lessor, wishlist, category) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var existingProduct))
                    {
                        existingProduct = product;
                        existingProduct.UpdateImagesProduct(new List<Images>());
                        existingProduct.UpdateWishlistsProduct(new List<Wishlist>());
                        productDictionary.Add(product.Id, existingProduct);
                    }

                    if (image != null)
                    {
                        existingProduct.Images.Add(image);
                    }
                    // Add wishlist if not already added
                    if (wishlist != null && !existingProduct.Wishlists.Any(w => w.Id == wishlist.Id))
                    {
                        existingProduct.Wishlists.Add(wishlist);
                    }
                    existingProduct.UpdateLessorProduct(lessor);
                    existingProduct.UpdateCategory(category);
                    return existingProduct;
                },
                parameters,
                splitOn: "ProductModifiedDate, ImageCreatedDate, LessorCreatedDate, WishlistModifiedDate"
            );

            // Return paginated result
            return new PagedResult<Product>(
                productDictionary.Values.ToList(),
                pageIndex,
                pageSize,
                totalCount,
                totalPages
            );
        }
    }


    public Task<int> UpdateAsync(Product entity)
    {
        throw new NotImplementedException();
    }
}
