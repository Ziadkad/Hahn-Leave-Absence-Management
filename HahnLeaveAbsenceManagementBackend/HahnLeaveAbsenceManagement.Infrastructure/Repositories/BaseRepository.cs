using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HahnLeaveAbsenceManagement.Infrastructure.Repositories;


public class BaseRepository<T, TKey>(AppDbContext dbContext) : IBaseRepository<T, TKey> where T : class
{
    protected DbSet<T> DbSet { get; } = dbContext.Set<T>();

    public virtual Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => DbSet.ToListAsync(cancellationToken).ContinueWith(task => task.Result.AsEnumerable(), cancellationToken);

    public virtual ValueTask<T?> FindAsync(TKey key, CancellationToken cancellationToken = default)
        => DbSet.FindAsync([key], cancellationToken);

    public virtual Task AddAsync(T newEntity, CancellationToken cancellationToken = default) 
        => DbSet.AddAsync(newEntity, cancellationToken).AsTask();

    public virtual void Update(T newEntity) => DbSet.Update(newEntity);

    public virtual void Remove(T newEntity) => DbSet.Remove(newEntity);
}