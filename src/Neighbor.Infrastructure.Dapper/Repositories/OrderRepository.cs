using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Domain.Entities;
using System.Text;

namespace Neighbor.Infrastructure.Dapper.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IConfiguration _configuration;
    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<int> AddAsync(Order entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(Order entity)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Order>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Order>? GetByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public async Task<Order> GetDetailsAsync(Guid orderId)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();

            // Query the product
            var orders = await connection.QueryAsync<Order, Account, Product, Lessor, Category, Order>(
                @"SELECT o.Id, o.RentTime, o.ReturnTime, o.DeliveryAddress, o.OrderValue, o.OrderStatus, o.OrderReportStatus, o.UserReasonReject, o.LessorReasonReject, o.UserReport, o.AdminReasonReject, o.CreatedDate, o.ModifiedDate AS OrderModifiedDate, a.Id, a.FirstName, a.LastName, a.Email, a.PhoneNumber, a.CropAvatarUrl, a.FullAvatarUrl, a.LoginType AS AccountLoginType, p.Id, p.Name, p.StatusType, p.Policies, p.Description, p.RejectReason, p.Rating, p.Price, p.Value, p.MaximumRentDays, p.ConfirmStatus, p.LessorId, p.CreatedDate, p.ModifiedDate AS ProductModifiedDate, l.Id, l.WareHouseAddress, l.ShopName, l.AccountId, l.CreatedDate AS LessorCreatedDate, c.Id, c.Name, c.IsVehicle
              FROM Orders o
              JOIN Accounts a ON a.Id = o.AccountId
              JOIN Products p ON p.Id = o.ProductId
              JOIN Lessors l ON l.Id = p.LessorId
              JOIN Categories c ON c.Id = p.CategoryId
              WHERE o.Id = @Id",
                (o, a, p, l, c) =>
                {
                    p.UpdateLessorProduct(l);
                    p.UpdateCategory(c);
                    o.UpdateProductOrder(p);
                    o.UpdateUserOrder(a);
                    return o;
                },
                new { Id = orderId },
                splitOn: "OrderModifiedDate, AccountLoginType, ProductModifiedDate, LessorCreatedDate");

            if (orders == null) return null;
            var order = orders.ToList()[0];
            var product = order.Product;
            // Query related images
            var productImages = await connection.QueryAsync<Images>(
                @"SELECT ImageLink, ImageId, CreatedDate FROM Images WHERE ProductId = @Id", new { Id = product.Id });

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
                new { Id = product.Id },
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
                new { Id = product.Id },
                splitOn: "InsuranceCreatedDate");

            // Update the product's insurance list
            product.UpdateInsurance(insuranceData.Values.ToList());

            return order;

        }
    }

    public async Task<PagedResult<Order>> GetPagedAsync(int pageIndex, int pageSize, Filter.OrderFilter filterParams, string[] selectedColumns)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            // Valid columns for selecting
            var validColumns = new HashSet<string>
        {
            "o.Id", "o.RentTime", "o.ReturnTime", "o.DeliveryAddress", "o.OrderValue", "o.OrderStatus", "o.OrderReportStatus", "o.UserReasonReject", "o.LessorReasonReject", "o.UserReport", "o.AdminReasonReject", "o.CreatedDate", "o.ModifiedDate AS OrderModifiedDate", "a.Id", "a.FirstName", "a.LastName", "a.Email", "a.PhoneNumber", "a.CropAvatarUrl", "a.FullAvatarUrl", "a.LoginType AS AccountLoginType",
            "p.Id", "p.Name", "p.StatusType", "p.Policies", "p.Description", "p.RejectReason",
            "p.Rating", "p.Price", "p.Value", "p.MaximumRentDays", "p.ConfirmStatus", "p.LessorId", "p.CreatedDate",
            "p.ModifiedDate AS ProductModifiedDate", "l.Id", "l.WareHouseAddress", "l.ShopName", "l.AccountId", "l.CreatedDate AS LessorCreatedDate", "c.Id", "c.Name", "c.IsVehicle"
        };

            var columns = selectedColumns?.Where(c => validColumns.Contains(c)).ToArray();

            // If no selected columns, select all
            var selectedColumnsString = columns?.Length > 0
                ? string.Join(", ", columns)
                : string.Join(", ", validColumns);

            // Base query for fetching data
            var queryBuilder = new StringBuilder($@"
            SELECT {selectedColumnsString} 
            FROM Orders o
            JOIN Accounts a ON a.Id = o.AccountId
            JOIN Products p ON p.Id = o.ProductId
            JOIN Lessors l ON l.Id = p.LessorId
            JOIN Categories c ON c.Id = p.CategoryId
            WHERE 1=1");

            // Base query for total count
            var totalCountQuery = new StringBuilder($@"
            SELECT COUNT(DISTINCT o.Id) 
            FROM Orders o
            JOIN Accounts a ON a.Id = o.AccountId
            JOIN Products p ON p.Id = o.ProductId            
            JOIN Lessors l ON l.Id = p.LessorId
            JOIN Categories c ON c.Id = p.CategoryId
            WHERE 1=1");

            var parameters = new DynamicParameters();

            // Pagination and defaults
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize > 100 ? 100 : pageSize;

            // Apply filters
            if (!string.IsNullOrEmpty(filterParams?.DeliveryAddress))
            {
                queryBuilder.Append(" AND o.DeliveryAddress LIKE @DeliveryAddress");
                totalCountQuery.Append(" AND o.DeliveryAddress LIKE @DeliveryAddress");
                parameters.Add("DeliveryAddress", $"%{filterParams.DeliveryAddress}%");
            }

            if (filterParams?.OrderStatus.HasValue == true)
            {
                queryBuilder.Append(" AND o.OrderStatus = @OrderStatus");
                totalCountQuery.Append(" AND o.OrderStatus = @OrderStatus");
                parameters.Add("OrderStatus", filterParams.OrderStatus);
            }

            if (filterParams?.OrderReportStatus.HasValue == true)
            {
                queryBuilder.Append(" AND o.OrderReportStatus = @OrderReportStatus");
                totalCountQuery.Append(" AND o.OrderReportStatus = @OrderReportStatus");
                parameters.Add("OrderReportStatus", filterParams.OrderReportStatus);
            }

            if (filterParams?.MinValue.HasValue == true && filterParams?.MaxValue.HasValue == true)
            {
                queryBuilder.Append(" AND o.OrderValue <= @MaxValue AND o.OrderValue >= @MinValue");
                totalCountQuery.Append(" AND o.OrderValue <= @MaxValue AND o.OrderValue >= @MinValue");
                parameters.Add("MinValue", filterParams.MinValue);
                parameters.Add("MaxValue", filterParams.MaxValue);
            }

            if (filterParams?.AccountUserId.HasValue == true)
            {
                queryBuilder.Append(" AND o.AccountId = @AccountUserId");
                totalCountQuery.Append(" AND o.AccountId = @AccountUserId");
                parameters.Add("AccountUserId", filterParams.AccountUserId);
            }

            if (filterParams?.AccountLessorId.HasValue == true)
            {
                queryBuilder.Append(" AND l.AccountId = @AccountLessorId");
                totalCountQuery.Append(" AND l.AccountId = @AccountLessorId");
                parameters.Add("AccountLessorId", filterParams.AccountLessorId);
            }

            // Get total count and pages
            var totalCount = await connection.ExecuteScalarAsync<int>(totalCountQuery.ToString(), parameters);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Pagination logic
            var offset = (pageIndex - 1) * pageSize;

            // Dictionary for mapping products and their images
            var orderDictionary = new Dictionary<Guid, Order>();

            // Execute the query
            var items = await connection.QueryAsync<Order, Account, Product, Lessor, Category, Order>(
                queryBuilder.ToString(),
                (order, account, product, lessor, category) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;
                        orderDictionary.Add(order.Id, existingOrder);
                    }
                    product.UpdateLessorProduct(lessor);
                    product.UpdateCategory(category);
                    existingOrder.UpdateProductOrder(product);
                    existingOrder.UpdateUserOrder(account);
                    return existingOrder;
                },
                parameters,
                splitOn: "OrderModifiedDate, AccountLoginType, ProductModifiedDate, LessorCreatedDate"
            );

            // Apply sorting based on filterParams
            var result = orderDictionary.Values.ToList();
            if (filterParams?.SortType.HasValue == true && filterParams?.IsSortASC.HasValue == true)
            {
                string sortType = filterParams.IsSortASC.Value ? "ASC" : "DESC";
                //Check if IsSortCreatedDayASC == true then Sort Created Date ASC
                if (filterParams?.SortType == SortType.CreatedDate)
                {
                    result = filterParams.IsSortASC.Value ? result.OrderBy(o => o.CreatedDate).ToList() : result.OrderByDescending(o => o.CreatedDate).ToList();
                }
                else if (filterParams?.SortType == SortType.RentTime)
                {
                    result = filterParams.IsSortASC.Value ? result.OrderBy(o => o.RentTime).ToList() : result.OrderByDescending(o => o.RentTime).ToList();
                }
                else
                {
                    result = filterParams.IsSortASC.Value ? result.OrderBy(o => o.ReturnTime).ToList() : result.OrderByDescending(o => o.ReturnTime).ToList();
                }
            }
            else
            {
                result = result.OrderBy(o => o.Id).ToList();
            }

            // Apply pagination
            result = result.Skip(offset).Take(pageSize).ToList();

            //Find Images and then Merge Images to Product Of Order
            var productIds = orderDictionary.Values
            .Select(order => order.Product.Id)
            .Distinct()
            .ToList();

            var imagesQuery = @"
            SELECT i.*
            FROM Images i
            WHERE i.ProductId IN @ProductIds";

            var images = await connection.QueryAsync<Images>(imagesQuery, new { ProductIds = productIds });

            var imagesGroupedByProduct = images
            .GroupBy(img => img.ProductId)
            .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var order in result)
            {
                var product = order.Product;

                if (imagesGroupedByProduct.TryGetValue(product.Id, out var productImages))
                {
                    product.UpdateImagesProduct(productImages);
                }
                else
                {
                    product.UpdateImagesProduct(new List<Images>()); // No images for this product
                }
            }

            // Return paginated result
            return new PagedResult<Order>(
                result,
                pageIndex,
                pageSize,
                totalCount,
                totalPages
            );
        }
    }

    public Task<int> UpdateAsync(Order entity)
    {
        throw new NotImplementedException();
    }
}
