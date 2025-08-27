using HahnLeaveAbsenceManagement.Domain.User;

namespace HahnLeaveAbsenceManagement.Application.User.Models;

public record LoginResponse()
{
    public Guid Id {get; set;}
    public string Email { get; set; }
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public UserRole Role {get; set;}
    public int LeavesLeft { get; set; }
    public string? Token {get; set;}
    public DateTime? TokenExpiresAt {get; set;}
};