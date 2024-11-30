using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Categories;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Domain.Entities;
using System.Text;

namespace Neighbor.Infrastructure.Dapper.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly IConfiguration _configuration;
    public CategoryRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<int> AddAsync(Category entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(Category entity)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Category>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Category>? GetByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResult<Category>> GetPagedAsync(int pageIndex, int pageSize, Filter.CategoryFilter filterParams, string[] selectedColumns)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            // Valid columns for selecting
            var validColumns = new HashSet<string> { "Id", "Name", "IsVehicle", "Image" };
            var columns = selectedColumns?.Where(c => validColumns.Contains(c)).ToArray();

            // If no selected columns, select all
            var selectedColumnsString = columns?.Length > 0 ? string.Join(", ", columns) : "*";

            // Start building the query
            var queryBuilder = new StringBuilder($"SELECT {selectedColumnsString} FROM Categories WHERE 1=1");

            var parameters = new DynamicParameters();

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize > 100 ? 100 : pageSize;

            var totalCountQuery = new StringBuilder($@"
        SELECT COUNT(1) 
        FROM Categories c
        WHERE 1=1");

            // Filter by Id
            if (filterParams?.Id.HasValue == true)
            {
                queryBuilder.Append(" AND Id = @Id");
                totalCountQuery.Append(" AND Id = @Id");
                parameters.Add("Id", $"{filterParams.Id}");
            }                       

            // Filter by Name
            if (!string.IsNullOrEmpty(filterParams?.Name))
            {
                queryBuilder.Append(" AND Name LIKE @Name");
                totalCountQuery.Append(" AND Name LIKE @Name");
                parameters.Add("Name", $"%{filterParams.Name}%");
            }

            if (filterParams?.IsVehicle != null)
            {
                queryBuilder.Append(" AND IsVehicle = @IsVehicle");
                totalCountQuery.Append(" AND IsVehicle = @IsVehicle");
                parameters.Add("IsVehicle", $"{filterParams.IsVehicle}");
            }

            //Count TotalCount
            var totalCount = await connection.ExecuteScalarAsync<int>(totalCountQuery.ToString(), parameters);

            //Count TotalPages
            var totalPages = Math.Ceiling((totalCount / (double)pageSize));

            var offset = (pageIndex - 1) * pageSize;
            var paginatedQuery = $"{queryBuilder} ORDER BY Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            var items = (await connection.QueryAsync<Category>(paginatedQuery, parameters)).ToList();

            return new PagedResult<Category>(items, pageIndex, pageSize, totalCount, totalPages);

        }
    }

    public Task<int> UpdateAsync(Category entity)
    {
        throw new NotImplementedException();
    }
}
