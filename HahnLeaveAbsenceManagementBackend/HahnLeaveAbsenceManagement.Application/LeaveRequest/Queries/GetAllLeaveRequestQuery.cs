using AutoMapper;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Application.Common.Models;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using MediatR;

namespace HahnLeaveAbsenceManagement.Application.LeaveRequest.Queries;

public record GetAllLeaveRequestQuery(Guid? UserId,LeaveType? Type, DateTime? StartDateFrom, DateTime? StartDateTo, DateTime? EndDateFrom, DateTime? EndDateTo, LeaveStatus? Status, PageQueryRequest PageQuery) :IRequest<PageQueryResult<LeaveRequestWithUserDto>>;

public class
    GetAllLeaveRequestQueryHandler(ILeaveRequestRepository leaveRequestRepository, IMapper mapper) : IRequestHandler<GetAllLeaveRequestQuery, PageQueryResult<LeaveRequestWithUserDto>>
{
    public async Task<PageQueryResult<LeaveRequestWithUserDto>> Handle(GetAllLeaveRequestQuery request, CancellationToken cancellationToken)
    {
        var (count, leaveRequests) = await leaveRequestRepository.SearchAsync(            request.UserId,
            request.Type,
            request.StartDateFrom,
            request.StartDateTo,
            request.EndDateFrom,
            request.EndDateTo,
            request.Status,
            request.PageQuery,
            cancellationToken);
        var leaveRequestWithUserDto = mapper.Map<List<LeaveRequestWithUserDto>>(leaveRequests);
        return new PageQueryResult<LeaveRequestWithUserDto>(leaveRequestWithUserDto, count);
    }
}
