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

    public Task<bool>? AccountExistSystemAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> AddAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistSystemAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Product>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Account> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Product>? GetByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public async Task<Product> GetDetailsAsync(Guid productId)
    {
        var sql = @"
    SELECT
        p.Id, p.Name, p.StatusType, p.Policies, p.Description, p.RejectReason, p.Rating, p.Price, p.Value, p.MaximumRentDays, p.ConfirmStatus, p.LessorId, p.CreatedDate, p.ModifiedDate AS ProductModifiedDate,
        ip.ImageLink, ip.ImageId, ip.CreatedDate AS ImagesProductCreatedDate,
        l.Id, l.WareHouseAddress, l.ShopName, l.TimeUnitType AS LessorTimeUnit,
        ps.Price, ps.ProductId, ps.SurchargeId, ps.CreatedDate AS ProductSurchargeCreatedDate,
        s.Id, s.Name, s.Description, s.IsDeleted AS SurchargeIsDeleted,
        i.Id, i.Name, i.IssueDate, i.ExpirationDate, i.CreatedDate AS InsuranceCreatedDate,
        ii.ImageLink, ii.ImageId, ii.CreatedDate AS ImagesInsuranceCreatedDate
    FROM Products p
    LEFT JOIN Images ip ON p.Id = ip.ProductId
    JOIN Lessor l ON l.Id = p.LessorId
    LEFT JOIN ProductSurcharges ps ON p.Id = ps.ProductId
    LEFT JOIN Surcharges s ON s.Id = ps.SurchargeId
    LEFT JOIN Insurances i ON p.Id = i.ProductId
    LEFT JOIN Images ii ON i.Id = ii.InsuranceId
    WHERE p.Id = @Id";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();

            var productDictionary = new Dictionary<Guid, Product>();

            var result = await connection.QueryAsync<Product, Images, Lessor, ProductSurcharge, Surcharge, Insurance, Images, Product>(
                sql,
                (product, productImage, lessor, productSurcharge, surcharge, insurance, insuranceImage) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var existingProduct))
                    {
                        existingProduct = product;
                        existingProduct.UpdateImagesProduct(new List<Images>());
                        existingProduct.UpdateProductSurcharges(new List<ProductSurcharge>());
                        existingProduct.UpdateInsurance(new List<Insurance>());
                        productDictionary.Add(product.Id, existingProduct);
                    }

                    // Add product images without duplicates
                    if (productImage != null && !existingProduct.Images.Any(img => img.ImageId == productImage.ImageId))
                    {
                        existingProduct.Images.Add(productImage);
                    }

                    // Add surcharges
                    if (productSurcharge != null)
                    {
                        productSurcharge.UpdateSurcharge(surcharge); // Link surcharge details
                        existingProduct.ProductSurcharges.Add(productSurcharge);
                    }

                    // Add insurances and their images
                    if (insurance != null)
                    {
                        var listImagesInsurance = insurance.Images ?? new List<Images>();

                        // Ensure we add unique images for insurance
                        if (insuranceImage != null && !listImagesInsurance.Any(img => img.ImageId == insuranceImage.ImageId))
                        {
                            listImagesInsurance.Add(insuranceImage);
                        }

                        insurance.UpdateImagesInsurance(listImagesInsurance);
                        existingProduct.Insurances.Add(insurance);
                    }

                    // Assign lessor details
                    existingProduct.UpdateLessorProduct(lessor);

                    return existingProduct;
                },
                new { Id = productId },
                splitOn: "ProductModifiedDate,ImagesProductCreatedDate, LessorTimeUnit, ProductSurchargeCreatedDate, SurchargeIsDeleted, InsuranceCreatedDate"
            );

            return productDictionary.Values.FirstOrDefault();
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
            "p.ModifiedDate AS ProductModifiedDate", "i.ImageLink", "i.ImageId", "i.CreatedDate AS ImageCreatedDate", "l.Id", "l.WareHouseAddress", "l.ShopName"
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
            JOIN Lessor l ON l.Id = p.LessorId
            WHERE 1=1");

            // Base query for total count
            var totalCountQuery = new StringBuilder($@"
            SELECT COUNT(DISTINCT p.Id) 
            FROM Products p
            LEFT JOIN Images i ON p.Id = i.ProductId
            JOIN Lessor l ON l.Id = p.LessorId
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

            if (filterParams?.AccountId.HasValue == true)
            {
                queryBuilder.Append(" AND l.AccountId = @AccountId");
                totalCountQuery.Append(" AND l.AccountId = @AccountId");
                parameters.Add("AccountId", filterParams.AccountId);
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
            queryBuilder.Append($" ORDER BY p.Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");

            // Dictionary for mapping products and their images
            var productDictionary = new Dictionary<Guid, Product>();

            // Execute the query
            var items = await connection.QueryAsync<Product, Images, Lessor, Product>(
                queryBuilder.ToString(),
                (product, image, lessor) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var existingProduct))
                    {
                        existingProduct = product;
                        existingProduct.UpdateImagesProduct(new List<Images>());
                        productDictionary.Add(product.Id, existingProduct);
                    }

                    if (image != null)
                    {
                        existingProduct.Images.Add(image);
                    }
                    existingProduct.UpdateLessorProduct(lessor);
                    return existingProduct;
                },
                parameters,
                splitOn: "ProductModifiedDate, ImageCreatedDate"
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
