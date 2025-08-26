using System.Runtime.Serialization;

namespace HahnLeaveAbsenceManagement.Domain.LeaveRequest;

public enum LeaveStatus
{
    [EnumMember(Value = "Pending")]
    Pending,
    [EnumMember(Value = "Approved")]
    Approved,
    [EnumMember(Value = "Rejected")]
    Rejected,
    [EnumMember(Value = "Cancelled")]
    Cancelled,
}