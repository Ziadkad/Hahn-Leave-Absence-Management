using HahnLeaveAbsenceManagement.Domain.User;

namespace HahnLeaveAbsenceManagement.Application.Common.Interfaces;

public interface IUserContext
{
    Guid GetCurrentUserId();
    UserRole GetUserRole();
}