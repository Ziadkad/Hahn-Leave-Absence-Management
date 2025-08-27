using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
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

}