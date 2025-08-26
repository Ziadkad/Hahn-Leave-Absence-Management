using System.Runtime.Serialization;

namespace HahnLeaveAbsenceManagement.Domain.LeaveRequest;

public enum LeaveType
{
    [EnumMember(Value = "Vacation")]
    Vacation,
    [EnumMember(Value = "Sick")]
    Sick,
    [EnumMember(Value = "Unpaid")]
    Unpaid
}