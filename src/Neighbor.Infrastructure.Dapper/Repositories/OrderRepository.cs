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
            var orders = await connection.QueryAsync<Order, Product, Lessor, Category, Order>(
                @"SELECT o.Id, o.RentTime, o.ReturnTime, o.DeliveryAddress, o.OrderValue, o.OrderStatus, o.UserReasonReject, o.LessorReasonReject, o.IsConflict, o.CreatedDate, o.ModifiedDate AS OrderModifiedDate, p.Id, p.Name, p.StatusType, p.Policies, p.Description, p.RejectReason, p.Rating, p.Price, p.Value, p.MaximumRentDays, p.ConfirmStatus, p.LessorId, p.CreatedDate, p.ModifiedDate AS ProductModifiedDate, l.Id, l.WareHouseAddress, l.ShopName, l.AccountId, l.Description AS LessorDescription, c.Id, c.Name, c.IsVehicle
              FROM Orders o
              JOIN Products p ON p.Id = o.ProductId
              JOIN Lessors l ON l.Id = p.LessorId
              JOIN Categories c ON c.Id = p.CategoryId
              WHERE o.Id = @Id",
                (o, p, l, c) =>
                {

                    p.UpdateLessorProduct(l);
                    p.UpdateCategory(c);
                    o.UpdateProductOrder(p);
                    return o;
                },
                new { Id = orderId },
                splitOn: "OrderModifiedDate, ProductModifiedDate, LessorDescription");

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
            "o.Id", "o.RentTime", "o.ReturnTime", "o.DeliveryAddress", "o.OrderValue", "o.OrderStatus", "o.UserReasonReject", "o.LessorReasonReject", "o.IsConflict", "o.CreatedDate", "o.ModifiedDate AS OrderModifiedDate", "a.Id", "a.FirstName", "a.LastName", "a.Email", "a.PhoneNumber", "a.CropAvatarLink", "a.FullAvatarLink", "a.LoginType AS AccountLoginType",
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
            FROM Orders o
            JOIN Accounts a
            JOIN Products p ON p.Id = o.ProductId
            LEFT JOIN Images i ON p.Id = i.ProductId
            LEFT JOIN Wishlists w on p.Id = w.ProductId
            JOIN Lessors l ON l.Id = p.LessorId
            JOIN Categories c ON c.Id = p.CategoryId
            WHERE 1=1");

            // Base query for total count
            var totalCountQuery = new StringBuilder($@"
            SELECT COUNT(DISTINCT o.Id) 
            FROM Orders o
            JOIN Accounts a
            JOIN Products p ON p.Id = o.ProductId            
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
            if (!string.IsNullOrEmpty(filterParams?.DeliveryAddress))
            {
                queryBuilder.Append(" AND o.DeliveryAddress LIKE @DeliveryAddress");
                totalCountQuery.Append(" AND o.DeliveryAddress LIKE @DeliveryAddress");
                parameters.Add("DeliveryAddress", $"%{filterParams.DeliveryAddress}%");
            }

            if (filterParams?.OrderStatus != null)
            {
                queryBuilder.Append(" AND o.OrderStatus = @OrderStatus");
                totalCountQuery.Append(" AND o.OrderStatus = @OrderStatus");
                parameters.Add("OrderStatus", filterParams.OrderStatus);
            }

            if (filterParams?.IsConflict.HasValue == true)
            {
                queryBuilder.Append(" AND o.IsConflict = @IsConflict");
                totalCountQuery.Append(" AND o.IsConflict = @IsConflict");
                parameters.Add("IsConflict", filterParams.IsConflict);
            }

            // Get total count and pages
            var totalCount = await connection.ExecuteScalarAsync<int>(totalCountQuery.ToString(), parameters);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Pagination logic
            var offset = (pageIndex - 1) * pageSize;
            //Check if IsSortCreatedDayASC exist then Sort by CreatedDate
            if (filterParams?.SortBy != null)
            {
                string sortType = filterParams.SortBy.IsSortASC ? "ASC" : "DESC";
                //Check if IsSortCreatedDayASC == true then Sort Created Date ASC
                if (filterParams?.SortBy.SortType == SortType.CreatedDate)
                {
                    queryBuilder.Append($" ORDER BY o.CreatedDate {sortType} OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
                }
                else if (filterParams?.SortBy.SortType == SortType.RentTime)
                {
                    queryBuilder.Append($" ORDER BY o.RentTime {sortType} DESC OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
                }
                else
                {
                    queryBuilder.Append($" ORDER BY o.ReturnTime {sortType} DESC OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
                }
            }
            else
            {
                queryBuilder.Append($" ORDER BY o.Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");
            }

            // Dictionary for mapping products and their images
            var orderDictionary = new Dictionary<Guid, Order>();

            // Execute the query
            var items = await connection.QueryAsync<Order, Account, Product, Images, Lessor, Wishlist, Category, Order>(
                queryBuilder.ToString(),
                (order, account, product, image, lessor, wishlist, category) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;

                        product.UpdateImagesProduct(new List<Images>());
                        product.UpdateWishlistsProduct(new List<Wishlist>());
                        orderDictionary.Add(order.Id, existingOrder);
                    }

                    // Add image if not already added
                    if (image != null && !product.Images.Any(img => img.ImageId == image.ImageId))
                    {
                        product.Images.Add(image);
                    }

                    product.UpdateLessorProduct(lessor);
                    product.UpdateCategory(category);
                    existingOrder.UpdateProductOrder(product);
                    existingOrder.UpdateUserOrder(account);
                    return existingOrder;
                },
                parameters,
                splitOn: "OrderModifiedDate, AccountLoginType, ProductModifiedDate, ImageCreatedDate, LessorCreatedDate, WishlistModifiedDate"
            );

            // Return paginated result
            return new PagedResult<Order>(
                orderDictionary.Values.ToList(),
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
