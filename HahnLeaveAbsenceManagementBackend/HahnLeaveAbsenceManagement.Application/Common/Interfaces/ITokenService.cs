namespace HahnLeaveAbsenceManagement.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(Domain.User.User user);
}