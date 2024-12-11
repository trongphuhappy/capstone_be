using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Infrastructure.Dapper.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly IConfiguration _configuration;
    public WalletRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public Task<int> AddAsync(Wallet entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(Wallet entity)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Wallet>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Wallet>? GetByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public async Task<Wallet> GetWalletByLessorId(Guid lessorId, string[] selectedColumns = null)
    {
        var validColumns = new HashSet<string>
        {
            "l.Id",
            "l.LessorId",
            "l.Balance",
        };


        // Lọc các cột hợp lệ
        var columns = selectedColumns?.Where(c => validColumns.Contains(c)).ToArray();
        var selectedColumnsString = columns?.Length > 0
            ? string.Join(", ", columns)
            : string.Join(", ", validColumns);

        var sql = @$"
        SELECT {selectedColumnsString}
        FROM Wallets l
        WHERE l.LessorId = @LessorId";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();

            var result = await connection.QueryAsync<Wallet>(
                sql,
                new { LessorId = lessorId }
                );

            return result.FirstOrDefault();
        }
    }

    public Task<int> UpdateAsync(Wallet entity)
    {
        throw new NotImplementedException();
    }
}
