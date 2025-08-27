using AutoMapper;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Application.User.Models;
using MediatR;

namespace HahnLeaveAbsenceManagement.Application.User.Queries;

public class GetAllUsersQuery() : IRequest<List<UserDto>>;

public class GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        List<Domain.User.User> users = (await userRepository.GetAllAsync(cancellationToken)).ToList();
        return mapper.Map<List<UserDto>>(users);
    }
}
