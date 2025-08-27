using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Application.Common.Models;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using HahnLeaveAbsenceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HahnLeaveAbsenceManagement.Infrastructure.Repositories;

public class LeaveRequestRepository(AppDbContext dbContext) : BaseRepository<LeaveRequest, Guid>(dbContext), ILeaveRequestRepository
{
    public async Task<bool> HasOverlappingLeaveAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return await dbContext.Set<LeaveRequest>()
            .AnyAsync(lr =>
                    lr.UserId == userId &&
                    lr.Status != LeaveStatus.Rejected &&
                    lr.Status != LeaveStatus.Cancelled &&
                    lr.StartDate <= endDate &&
                    lr.EndDate >= startDate,
                cancellationToken);
    }

    public async Task<(int totalCount, List<LeaveRequest> leaveRequests)> SearchAsync(Guid? userId, LeaveType? type, DateTime? startDateFrom, DateTime? startDateTo, DateTime? endDateFrom,
        DateTime? endDateTo, LeaveStatus? status, PageQueryRequest pageQuery,
        CancellationToken cancellationToken = default)
    {
        IQueryable<LeaveRequest> query = dbContext.Set<LeaveRequest>()
            .AsNoTracking()
            .Include(x => x.User);

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value);

        if (type.HasValue)
            query = query.Where(x => x.Type == type.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (startDateFrom.HasValue)
            query = query.Where(x => x.StartDate >= startDateFrom.Value);

        if (startDateTo.HasValue)
            query = query.Where(x => x.StartDate <= startDateTo.Value);

        if (endDateFrom.HasValue)
            query = query.Where(x => x.EndDate >= endDateFrom.Value);

        if (endDateTo.HasValue)
            query = query.Where(x => x.EndDate <= endDateTo.Value);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var leaveRequests = await query
            .Skip((pageQuery.Page - 1) * pageQuery.PageSize)
            .Take(pageQuery.PageSize)
            .ToListAsync(cancellationToken);

        return (totalCount, leaveRequests);
    }
}