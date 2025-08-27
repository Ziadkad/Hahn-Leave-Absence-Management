using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HahnLeaveAbsenceManagement.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IUserContext userContext) : DbContext(options),IUnitOfWork
{
    private bool _skipAudit = false;

    public IDisposable TemporarilySkipAudit()
    {
        _skipAudit = true;
        return new RestoreAuditFlag(this);
    }

    private class RestoreAuditFlag : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly bool _originalValue;

        public RestoreAuditFlag(AppDbContext context)
        {
            _context = context;
            _originalValue = context._skipAudit;
        }

        public void Dispose()
        {
            _context._skipAudit = false;
        }
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!_skipAudit)
        {
            foreach (var entry in ChangeTracker.Entries<BaseModel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateAudit(userContext.GetCurrentUserId());
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdateAudit(userContext.GetCurrentUserId());
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        throw new InvalidOperationException("Use SaveChangesAsync method instead of SaveChanges");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}