
namespace HahnLeaveAbsenceManagement.Application.Common.Interfaces;

public interface ILeaveRequestRepository : IBaseRepository<Domain.LeaveRequest.LeaveRequest, Guid>
{
    Task<bool> HasOverlappingLeaveAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}