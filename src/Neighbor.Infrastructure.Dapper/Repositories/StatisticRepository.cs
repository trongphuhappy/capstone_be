using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.StatisticDTOs;
using Neighbor.Domain.Abstraction.Dappers.Repositories;

namespace Neighbor.Infrastructure.Dapper.Repositories;

public class StatisticRepository : IStatisticRepository
{
    private readonly IConfiguration _configuration;
    public StatisticRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<OrderStatisticsDTO>> GetMonthlyOrderStatisticsByLessorAsync(Guid lessorId)
    {
        var query = @"
            WITH LatestYearOrders AS (
                SELECT 
                    YEAR(CreatedDate) AS OrderYear,
                    MONTH(CreatedDate) AS OrderMonth,
                    COUNT(Id) AS OrderCount
                FROM Orders
                WHERE LessorId = @LessorId
                GROUP BY YEAR(CreatedDate), MONTH(CreatedDate)
            )
            SELECT 
                OrderMonth,
                OrderCount
            FROM LatestYearOrders
            WHERE OrderYear = (SELECT MAX(YEAR(CreatedDate)) FROM Orders WHERE LessorId = @LessorId)
            ORDER BY OrderMonth;
        ";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<OrderStatisticsDTO>(query, new { LessorId = lessorId });
        }
    }

    public async Task<List<BoxLessorDTO>> GetTotalBoxLessorAsync(Guid lessorId)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();

            // Get total product of this month and last month
            string queryGetTotalProducts = @"
            SELECT
                COUNT(CASE WHEN MONTH(CreatedDate) = MONTH(GETDATE()) AND YEAR(CreatedDate) = YEAR(GETDATE()) THEN 1 END) AS ValueThisMonth,
                COUNT(CASE WHEN MONTH(CreatedDate) = MONTH(DATEADD(MONTH, -1, GETDATE())) AND YEAR(CreatedDate) = YEAR(DATEADD(MONTH, -1, GETDATE())) THEN 1 END) AS ValueLastMonth
            FROM Products
            WHERE LessorId = @LessorId";

            var resultGetTotalProducts = await connection.QueryFirstOrDefaultAsync<BoxLessorDTO>(queryGetTotalProducts, new { LessorId = lessorId });

            // Get total order value of this month and last month
            string queryGetTotalOrderValues = @"
            SELECT
                SUM(CASE WHEN MONTH(CreatedDate) = MONTH(GETDATE()) AND YEAR(CreatedDate) = YEAR(GETDATE()) THEN OrderValue ELSE 0 END) AS ValueThisMonth,
                SUM(CASE WHEN MONTH(CreatedDate) = MONTH(DATEADD(MONTH, -1, GETDATE())) AND YEAR(CreatedDate) = YEAR(DATEADD(MONTH, -1, GETDATE())) THEN OrderValue ELSE 0 END) AS ValueLastMonth
            FROM Orders
            WHERE LessorId = @LessorId AND (OrderStatus = 2 OR OrderReportStatus = -1)";

            var resultGetTotalOrderValues = await connection.QueryFirstOrDefaultAsync<BoxLessorDTO>(queryGetTotalOrderValues, new { LessorId = lessorId });

            // Get total order success
            string queryGetTotalOrdersSuccess = @"
            SELECT
                COUNT(CASE WHEN MONTH(CreatedDate) = MONTH(GETDATE()) AND YEAR(CreatedDate) = YEAR(GETDATE()) THEN 1 END) AS ValueThisMonth,
                COUNT(CASE WHEN MONTH(CreatedDate) = MONTH(DATEADD(MONTH, -1, GETDATE())) AND YEAR(CreatedDate) = YEAR(DATEADD(MONTH, -1, GETDATE())) THEN 1 END) AS ValueLastMonth
            FROM Orders
            WHERE LessorId = @LessorId AND OrderStatus = 2";

            var resultGetTotalOrderSuccess = await connection.QueryFirstOrDefaultAsync<BoxLessorDTO>(queryGetTotalOrdersSuccess, new { LessorId = lessorId });
            
            var listTotal = new List<BoxLessorDTO>() { resultGetTotalProducts, resultGetTotalOrderValues, resultGetTotalOrderSuccess};
            return listTotal;
        }
    }

    public async Task<OrderStatsDTO> GetPercentTotalOrder(Guid lessorId)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionStrings")))
        {
            await connection.OpenAsync();

            string query = @"
                SELECT
                    COUNT(*) AS TotalOrders,
                    CASE 
                        WHEN COUNT(*) > 0 THEN SUM(CASE WHEN OrderStatus = 2 THEN 1 ELSE 0 END) * 100.0 / COUNT(*)
                        ELSE 0 
                    END AS PercentOrderSuccess,
                    CASE 
                        WHEN COUNT(*) > 0 THEN SUM(CASE WHEN OrderStatus IN (-1, -3) THEN 1 ELSE 0 END) * 100.0 / COUNT(*)
                        ELSE 0 
                    END AS PercentOrderFail
                FROM Orders
                WHERE LessorId = @LessorId";

            var result = await connection.QueryFirstOrDefaultAsync<OrderStatsDTO>(query, new { LessorId = lessorId });

            return result;
        }
    }
}
