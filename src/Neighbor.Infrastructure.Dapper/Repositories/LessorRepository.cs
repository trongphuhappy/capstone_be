using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Infrastructure.Dapper.Repositories;
public class LessorRepository : ILessorRepository
{
    private readonly IConfiguration _configuration;
    public LessorRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public Task<int> AddAsync(Lessor entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(Lessor entity)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Lessor>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Lessor>? GetByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public async Task<Lessor> GetLessorByUserIdAsync(Guid userId)
    {
        var sql = @"
        SELECT l.Id, l.WareHouseAddress, l.Description, l.ShopName, l.LocationType, l.AccountId
        FROM Lessors l
        WHERE l.AccountId = @UserId";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();

            var result = await connection.QueryAsync<Lessor>(
                sql,
                new { UserId = userId }
                );

            return result.FirstOrDefault();
        }
    }

    public async Task<bool>? LessorExistByAccountIdAsync(Guid userId)
    {
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Lessors WHERE AccountId = @UserId) THEN 1 ELSE 0 END";
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();
            var result = await connection.ExecuteScalarAsync<bool>(sql, new { UserId = userId });
            return result;
        }
    }

    public Task<int> UpdateAsync(Lessor entity)
    {
        throw new NotImplementedException();
    }
}
