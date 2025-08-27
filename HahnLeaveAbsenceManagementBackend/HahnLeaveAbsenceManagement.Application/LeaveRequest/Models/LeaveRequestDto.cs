using HahnLeaveAbsenceManagement.Domain.LeaveRequest;

namespace HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;

public record LeaveRequestDto
{
    public Guid Id { get; set; }
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int BusinessDays { get; set; }
    public LeaveStatus Status { get; set; }
    public string Description { get; set; }
};