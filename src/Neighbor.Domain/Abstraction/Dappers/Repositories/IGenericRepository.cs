namespace Neighbor.Domain.Abstraction.Dappers.Repositories;
public interface IGenericRepository<T> where T : class
{
    Task<T>? GetByIdAsync(Guid Id);
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<int> AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(T entity);
}
