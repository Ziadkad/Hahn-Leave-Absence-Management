namespace HahnLeaveAbsenceManagement.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IDisposable TemporarilySkipAudit();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}