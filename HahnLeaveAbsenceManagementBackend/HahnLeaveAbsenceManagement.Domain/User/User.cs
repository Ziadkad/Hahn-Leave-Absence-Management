using HahnLeaveAbsenceManagement.Domain.Common;

namespace HahnLeaveAbsenceManagement.Domain.User;

public class User : BaseModel
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserRole Role { get; private set; }
    
    public List<LeaveRequest.LeaveRequest> LeaveRequests { get; private set; }

    public User(string email, string password, string firstName, string lastName, UserRole role)
    {
        Id = Guid.NewGuid();
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }
}