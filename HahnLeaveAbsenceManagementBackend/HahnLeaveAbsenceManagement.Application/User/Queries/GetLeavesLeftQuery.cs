using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using MediatR;

namespace HahnLeaveAbsenceManagement.Application.User.Queries;

public record GetLeavesLeftQuery : IRequest<int>;


public class GetLeavesLeftQueryHandler(IUserRepository userRepository, IUserContext userContext) : IRequestHandler<GetLeavesLeftQuery, int>
{
    public async Task<int> Handle(GetLeavesLeftQuery request, CancellationToken cancellationToken)
    {
        Guid userId = userContext.GetCurrentUserId();
        Domain.User.User? user = await userRepository.FindAsync(userId, cancellationToken);
    if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }
        return user.LeavesLeft;
    }
}

