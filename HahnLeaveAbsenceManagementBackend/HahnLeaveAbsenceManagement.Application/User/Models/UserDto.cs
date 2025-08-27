using HahnLeaveAbsenceManagement.Domain.User;

namespace HahnLeaveAbsenceManagement.Application.User.Models;

public class UserDto
{
    public Guid Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public UserRole Role {get; set;}
    public int LeavesLeft { get; set; }
 
}