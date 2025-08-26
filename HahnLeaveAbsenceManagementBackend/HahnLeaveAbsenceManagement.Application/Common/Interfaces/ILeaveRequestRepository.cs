using HahnLeaveAbsenceManagement.Domain.LeaveRequest;

namespace HahnLeaveAbsenceManagement.Application.Common.Interfaces;

public interface ILeaveRequestRepository : IBaseRepository<LeaveRequest, Guid>
{
    
}