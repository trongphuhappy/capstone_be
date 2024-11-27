using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Domain.Abstraction.Dappers.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Infrastructure.Dapper.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly IConfiguration _configuration;
        public WishlistRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<int> AddAsync(Wishlist entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Wishlist entity)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Wishlist>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Wishlist>? GetByIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsProductExistInWishlist(Guid AccountId, Guid ProductId)
        {
            var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Wishlists WHERE AccountId = @AccountId AND ProductId = @ProductId) THEN 1 ELSE 0 END";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
            {
                await connection.OpenAsync();
                var result = await connection.ExecuteScalarAsync<bool>(sql, new { AccountId, ProductId });
                return result;
            }
        }

        public Task<int> UpdateAsync(Wishlist entity)
        {
            throw new NotImplementedException();
        }
    }
}
