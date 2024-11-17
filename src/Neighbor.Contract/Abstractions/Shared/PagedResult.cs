using Dapper;
using System.Data;

namespace Neighbor.Contract.Abstractions.Shared;

public sealed class PagedResult<T>
{
    public const int UpperPageSize = 100;
    public const int DefaultPageSize = 10;
    public const int DefaultPageIndex = 1;

    public PagedResult(List<T> items, int pageIndex, int pageSize, int totalCount, double totalPages)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
    }

    public List<T> Items { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public double TotalPages { get; }
    public bool HasNextPage => PageIndex * PageSize < TotalCount;
    public bool HasPreviousPage => PageIndex > 1;

    public static async Task<PagedResult<T>> CreateAsync(IDbConnection dbConnection, string query, object parameters, int pageIndex, int pageSize)
    {
        pageIndex = pageIndex <= 0 ? DefaultPageIndex : pageIndex;
        pageSize = pageSize <= 0 ? DefaultPageSize : pageSize > UpperPageSize ? UpperPageSize : pageSize;

        // Total record
        var totalCountQuery = $"SELECT COUNT(1) FROM ({query}) AS CountQuery";
        var totalCount = await dbConnection.ExecuteScalarAsync<int>(totalCountQuery, parameters);

        // Get data
        var paginatedQuery = $"{query} ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        var paginatedParameters = new DynamicParameters(parameters);
        paginatedParameters.Add("Offset", (pageIndex - 1) * pageSize);
        paginatedParameters.Add("PageSize", pageSize);

        var items = (await dbConnection.QueryAsync<T>(paginatedQuery, paginatedParameters)).ToList();

        var totalPages = 0;

        return new PagedResult<T>(items, pageIndex, pageSize, totalCount, totalPages);
    }
}
