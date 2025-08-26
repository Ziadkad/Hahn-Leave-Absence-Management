using System.Runtime.Serialization;

namespace HahnLeaveAbsenceManagement.Domain.User;

public enum UserRole
{
    [EnumMember(Value = "HumanResourcesManager")]
    HumanResourcesManager,
    
    [EnumMember(Value = "Employee")]
    Employee
    
}