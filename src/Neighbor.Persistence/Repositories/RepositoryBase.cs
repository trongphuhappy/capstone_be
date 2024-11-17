using Microsoft.EntityFrameworkCore;
using Neighbor.Domain.Abstraction.Entities;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using System.Linq.Expressions;

namespace Neighbor.Persistence.Repositories;
public class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>, IDisposable
     where TEntity : DomainEntity<TKey>
{

    private readonly ApplicationDbContext _context;

    public RepositoryBase(ApplicationDbContext context)
        => _context = context;

    public void Dispose()
        => _context?.Dispose();

    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> items = _context.Set<TEntity>().AsNoTracking(); // Importance Always include AsNoTracking for Query Side
        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);

        if (predicate is not null)
            items = items.Where(predicate);

        return items;
    }

    public async Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties).AsTracking().SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    public async Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties).AsTracking().SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> items = _context.Set<TEntity>().AsNoTracking(); // Importance Always include AsNoTracking for Query Side
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        }
        if (predicate != null)
        {
            items = items.Where(predicate);
        }

        return await items.AnyAsync(cancellationToken);
    }

    public void Add(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedDate = DateTime.UtcNow;
        _context.Add(entity);
    }

    public void Remove(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public void RemoveMultiple(List<TEntity> entities)
        => _context.Set<TEntity>().RemoveRange(entities);

    public void Update(TEntity entity)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        _context.Set<TEntity>().Update(entity);
    }

    public void AddRange(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
        }
        _context.AddRange(entities);
    }
}

