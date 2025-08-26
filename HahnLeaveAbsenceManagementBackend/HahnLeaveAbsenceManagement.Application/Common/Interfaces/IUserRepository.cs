
namespace HahnLeaveAbsenceManagement.Application.Common.Interfaces;

public interface IUserRepository : IBaseRepository<Domain.User.User,Guid>
{
    Task<Domain.User.User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}