using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Infrastructure.Dapper.Repositories;
public class AccountRepository : IAccountRepository
{
    private readonly IConfiguration _configuration;
    public AccountRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool>? AccountExistSystemAsync(Guid userId)
    {
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Accounts WHERE Id = @Id) THEN 1 ELSE 0 END";
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();
            var result = await connection.ExecuteScalarAsync<bool>(sql, new { Id = userId });
            return result;
        }
    }

    public Task<int> AddAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> EmailExistSystemAsync(string email)
    {
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Accounts WHERE Email = @Email) THEN 1 ELSE 0 END";
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();
            var result = await connection.ExecuteScalarAsync<bool>(sql, new { Email = email });
            return result;
        }
    }

    public Task<IReadOnlyCollection<Account>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Account> GetByEmailAsync(string email)
    {
        var sql = "SELECT Id, FirstName, LastName, Email, PhoneNumber, Password, RoleUserId, LoginType, CropAvatarUrl, FullAvatarUrl, IsDeleted FROM Accounts WHERE Email = @Email";
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();
            var result = await connection.QuerySingleOrDefaultAsync<Account>(sql, new { Email = email });
            return result;
        }
    }

    public async Task<Account> GetByIdAsync(Guid id)
    {
        var sql = @"
        SELECT 
        [Id]
      ,[FirstName]
      ,[LastName]
      ,[Email]
      ,[PhoneNumber]
      ,[Password]
      ,[Biography]
      ,[CropAvatarUrl]
      ,[CropAvatarId]
      ,[FullAvatarUrl]
      ,[FullAvatarId]
      ,[CropCoverPhotoId]
      ,[CropCoverPhotoUrl]
      ,[FullCoverPhotoId]
      ,[FullCoverPhotoUrl]
      ,[LoginType]
      ,[GenderType]
      ,[RoleUserId]
      ,[CitizenId]
      ,[CreatedDate]
      ,[ModifiedDate]
      ,[IsDeleted]
        FROM [Accounts]
        WHERE [Id] = @id";
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();
            var result = await connection.QuerySingleOrDefaultAsync<Account>(sql, new { Id = id });
            return result;
        }
    }

    public Task<int> UpdateAsync(Account entity)
    {
        throw new NotImplementedException();
    }
}
