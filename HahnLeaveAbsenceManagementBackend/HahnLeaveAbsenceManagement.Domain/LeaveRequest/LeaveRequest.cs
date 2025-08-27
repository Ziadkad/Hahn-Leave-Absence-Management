
using System.Data;

namespace HahnLeaveAbsenceManagement.Domain.LeaveRequest;

public class LeaveRequest
{
    public Guid Id { get; private set; }
    public LeaveType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int BusinessDays { get; private set; }
    public LeaveStatus Status { get; private set; }
    public string Description { get; private set; } = string.Empty;
    
    public Guid UserId { get; private set; }
    public User.User User { get; private set; }

    public LeaveRequest(LeaveType type, DateTime startDate, DateTime endDate, string description, int businessDays, Guid userId)
    {
        Id = Guid.NewGuid();
        Type = type;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        BusinessDays = businessDays;
        UserId = userId;
        Status = LeaveStatus.Pending;
    }

    public void UpdateStatus(LeaveStatus status)
    {
        Status = status;
    }
}