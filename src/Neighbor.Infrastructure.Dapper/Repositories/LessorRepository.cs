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

    public async Task<Lessor> GetLessorByUserId(Guid userId)
    {

        var sql = @"
        SELECT l.Id, l.WareHouseAddress, l.Description, l.ShopName, l.ResponseRate, l.ResponseTime, l.AggreementRate, l.TimeUnitType, l.LocationType, l.AccountId
        FROM Lessors l
        WHERE l.UserId = @UserId";

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

    public Task<int> UpdateAsync(Lessor entity)
    {
        throw new NotImplementedException();
    }
}
