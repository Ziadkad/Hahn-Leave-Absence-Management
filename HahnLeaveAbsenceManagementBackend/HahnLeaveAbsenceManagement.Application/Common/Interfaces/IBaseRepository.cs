namespace HahnLeaveAbsenceManagement.Application.Common.Interfaces;

public interface IBaseRepository<T, TKey> 
    where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    ValueTask<T?> FindAsync(TKey key, CancellationToken cancellationToken = default);

    Task AddAsync(T newEntity, CancellationToken cancellationToken = default);

    void Update(T newEntity);

    void Remove(T newEntity);
}