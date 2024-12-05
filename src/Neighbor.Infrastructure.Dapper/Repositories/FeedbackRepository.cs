using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.FeedbackDTOs;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Services.Feedbacks;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Domain.Entities;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Text;

namespace Neighbor.Infrastructure.Dapper.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly IConfiguration _configuration;

    public FeedbackRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<int> AddAsync(Feedback entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(Feedback entity)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Feedback>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Feedback>? GetByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(Feedback entity)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResult<Feedback>> GetPagedAsync(
    int pageIndex,
    int pageSize,
    Filter.FeedbackFilter filterParams,
    string[] selectedColumns)
    {
        if (pageIndex <= 0 || pageSize <= 0)
            throw new ArgumentException("PageIndex and PageSize must be greater than zero.");

        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            var validColumns = new HashSet<string>
        {
            "f.Id", "f.Content", "f.[Like]", "f.OrderId", "f.ProductId","f.CreatedDate" ,"f.AccountId as AccountId",
            "a.FirstName", "a.LastName", "a.Email", "a.PhoneNumber",
            "a.CropAvatarUrl", "a.FullAvatarUrl"
        };

            // Lọc các cột hợp lệ
            var columns = selectedColumns?.Where(c => validColumns.Contains(c)).ToArray();
            var selectedColumnsString = columns?.Length > 0
                ? string.Join(", ", columns)
                : string.Join(", ", validColumns);

            // Xây dựng câu truy vấn chính
            var offset = (pageIndex - 1) * pageSize;
            
            var queryBuilder = new StringBuilder($@"
            SELECT {selectedColumnsString} 
            FROM Feedbacks f
            JOIN Accounts a ON a.Id = f.AccountId
            WHERE 1=1");

            // Câu truy vấn tổng số bản ghi
            var totalCountQuery = new StringBuilder($@"
            SELECT COUNT(*)
            FROM Feedbacks f
            JOIN Accounts a ON a.Id = f.AccountId
            WHERE 1=1");

            // Áp dụng bộ lọc
            var parameters = new DynamicParameters();

            if (filterParams?.ProductId.HasValue == true)
            {
                queryBuilder.Append(" AND f.ProductId = @ProductId");
                totalCountQuery.Append(" AND f.ProductId = @ProductId");
                parameters.Add("ProductId", filterParams.ProductId);
            }

            // Thêm sắp xếp và phân trang
            queryBuilder.Append(" ORDER BY f.CreatedDate DESC");
            queryBuilder.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);


            // Thực thi truy vấn tổng số bản ghi
            var totalCount = await connection.ExecuteScalarAsync<int>(totalCountQuery.ToString(), parameters);

            // Thực thi truy vấn lấy dữ liệu
            var results = await connection.QueryAsync<Feedback, Account, Feedback>(
                queryBuilder.ToString(),
                (feedback, account) =>
                {
                    feedback.UpdateAccount(account);
                    return feedback;
                },
                parameters,
                splitOn: "AccountId"
            );

            // Trả về kết quả phân trang
            return new PagedResult<Feedback>(
                results.ToList(), // Chuyển sang List để đảm bảo tính tương thích
                pageIndex,
                pageSize,
                totalCount,
                Math.Ceiling((double)totalCount / pageSize) // Tổng số trang
            );
        }
    }

}
