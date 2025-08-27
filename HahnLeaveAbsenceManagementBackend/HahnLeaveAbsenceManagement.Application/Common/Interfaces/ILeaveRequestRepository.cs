using HahnLeaveAbsenceManagement.Application.Common.Models;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;

namespace HahnLeaveAbsenceManagement.Application.Common.Interfaces;

public interface ILeaveRequestRepository : IBaseRepository<Domain.LeaveRequest.LeaveRequest, Guid>
{
    Task<bool> HasOverlappingLeaveAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    
    Task<(int totalCount,List<Domain.LeaveRequest.LeaveRequest> leaveRequests)> SearchAsync(
        Guid? userId,
        LeaveType? type,
        DateTime? startDateFrom,
        DateTime? startDateTo,
        DateTime? endDateFrom,
        DateTime? endDateTo,
        LeaveStatus? status,
        PageQueryRequest pageQuery,
        CancellationToken cancellationToken = default);
}