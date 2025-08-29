using System.Security.Cryptography;
using System.Text;
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
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    // --- Deterministic helpers (to avoid migration churn) ---
    static Guid StableGuid(string ns, string name)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(ns + ":" + name);
        var hash = md5.ComputeHash(bytes);
        return new Guid(hash);
    }
    static bool IsWeekend(DateTime d) => d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    static DateTime AddBusinessDays(DateTime start, int businessDays)
    {
        var d = start.Date;
        var added = 0;
        while (added < businessDays)
        {
            d = d.AddDays(1);
            if (!IsWeekend(d)) added++;
        }
        return d;
    }
    static int CountBusinessDays(DateTime start, DateTime endInclusive)
    {
        if (endInclusive < start) return 0;
        var c = 0;
        for (var d = start.Date; d <= endInclusive.Date; d = d.AddDays(1))
            if (!IsWeekend(d)) c++;
        return c;
    }
    static DateTime RandomBizDayInMonth(int year, int month, Random rng)
    {
        var days = DateTime.DaysInMonth(year, month);
        while (true)
        {
            var d = new DateTime(year, month, rng.Next(1, days + 1));
            if (!IsWeekend(d)) return d;
        }
    }

    // --- Audit constants for seeding ---
    var seederUserId   = StableGuid("system", "seed"); // "system/seeder" creator for seeded rows
    var seedCreatedOn  = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc);

    // --- Users ---
    const string defaultPassword = "Password123!";
    const string fixedSalt = "$2a$10$7EqJtq98hPqEX7fNZaFWo.";
    var passwordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword, fixedSalt);

    var firstNames = new[] { "Amina","Youssef","Sara","Omar","Hajar","Reda","Nadia","Adil","Meriem","Karim","Salma","Yassin","Imane","Anas","Lina" };
    var lastNames  = new[] { "Bennani","ElFassi","Azli","Amrani","Bouazza","ElIdrissi","Zerhouni","Tazi","ElMalki","Chafik","Alaoui","Berrada","Mouline","Jebbar","Farissi" };

    var userSeed = new List<object>(15);
    for (int i = 0; i < 15; i++)
    {
        var id = StableGuid("user", i.ToString());
        var fn = firstNames[i % firstNames.Length];
        var ln = lastNames[i % lastNames.Length];
        var role = i < 4 ? Domain.User.UserRole.HumanResourcesManager : Domain.User.UserRole.Employee;

        userSeed.Add(new
        {
            Id = id,
            Email = $"{fn.ToLower()}.{ln.ToLower()}@hahn.local",
            Password = passwordHash,
            FirstName = fn,
            LastName = ln,
            LeavesLeft = 18,
            Role = role,

            // Audit
            CreatedOn = seedCreatedOn,
            CreatedBy = seederUserId
        });
    }
    modelBuilder.Entity<Domain.User.User>().HasData(userSeed.ToArray());

    // --- Leave Requests ---
    var leaveSeed = new List<object>(15 * 3);
    for (int i = 0; i < 15; i++)
    {
        var userId = (Guid)((dynamic)userSeed[i]).Id;
        var rng = new Random(1000 + i);

        // AUG 2025
        {
            var start = RandomBizDayInMonth(2025, 8, rng);
            var len = rng.Next(1, 6); // 1..5 business days
            var end = AddBusinessDays(start, len - 1);
            if (end.Month != 8 || end.Year != 2025)
            {
                var last = new DateTime(2025, 8, DateTime.DaysInMonth(2025, 8));
                while (IsWeekend(last)) last = last.AddDays(-1);
                end = last;
                len = CountBusinessDays(start, end);
            }
            var status = rng.Next(2) == 0 ? Domain.LeaveRequest.LeaveStatus.Pending : Domain.LeaveRequest.LeaveStatus.Approved;
            leaveSeed.Add(new
            {
                Id = StableGuid("leave", $"{i}-aug"),
                Type = (Domain.LeaveRequest.LeaveType)rng.Next(0, 3),
                StartDate = start,
                EndDate = end,
                BusinessDays = len,
                Status = status,
                Description = "",
                UserId = userId,

                // Audit: treat the requester as creator; created on request start date
                CreatedOn = start,
                CreatedBy = userId
            });
        }

        // SEP 2025
        {
            var start = RandomBizDayInMonth(2025, 9, rng);
            var len = rng.Next(1, 6);
            var end = AddBusinessDays(start, len - 1);
            if (end.Month != 9 || end.Year != 2025)
            {
                var last = new DateTime(2025, 9, DateTime.DaysInMonth(2025, 9));
                while (IsWeekend(last)) last = last.AddDays(-1);
                end = last;
                len = CountBusinessDays(start, end);
            }
            var status = rng.Next(2) == 0 ? Domain.LeaveRequest.LeaveStatus.Pending : Domain.LeaveRequest.LeaveStatus.Approved;
            leaveSeed.Add(new
            {
                Id = StableGuid("leave", $"{i}-sep"),
                Type = (Domain.LeaveRequest.LeaveType)rng.Next(0, 3),
                StartDate = start,
                EndDate = end,
                BusinessDays = len,
                Status = status,
                Description = "",
                UserId = userId,

                // Audit
                CreatedOn = start,
                CreatedBy = userId
            });
        }

        // PAST (< 2025-08-29)
        {
            var cutoff = new DateTime(2025, 8, 29);
            var min = new DateTime(2024, 1, 1);
            var maxStart = cutoff.AddMonths(-2);
            var range = (maxStart - min).Days;
            var start = min.AddDays(rng.Next(Math.Max(1, range))).Date;
            while (IsWeekend(start)) start = start.AddDays(1);

            var len = rng.Next(1, 6);
            var end = AddBusinessDays(start, len - 1);
            if (end >= cutoff) end = cutoff.AddDays(-1);
            while (IsWeekend(end)) end = end.AddDays(-1);
            len = CountBusinessDays(start, end);

            leaveSeed.Add(new
            {
                Id = StableGuid("leave", $"{i}-past"),
                Type = (Domain.LeaveRequest.LeaveType)rng.Next(0, 3),
                StartDate = start,
                EndDate = end,
                BusinessDays = len,
                Status = Domain.LeaveRequest.LeaveStatus.Approved,
                Description = "",
                UserId = userId,

                // Audit
                CreatedOn = start,
                CreatedBy = userId
            });
        }
    }
    modelBuilder.Entity<Domain.LeaveRequest.LeaveRequest>().HasData(leaveSeed.ToArray());
}

}