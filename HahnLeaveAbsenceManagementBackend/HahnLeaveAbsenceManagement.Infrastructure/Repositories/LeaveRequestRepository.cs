using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using HahnLeaveAbsenceManagement.Infrastructure.Data;

namespace HahnLeaveAbsenceManagement.Infrastructure.Repositories;

public class LeaveRequestRepository(AppDbContext dbContext) : BaseRepository<LeaveRequest, Guid>(dbContext), ILeaveRequestRepository
{
    
}