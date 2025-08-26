using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HahnLeaveAbsenceManagement.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IUserContext userContext) : DbContext(options),IUnitOfWork
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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