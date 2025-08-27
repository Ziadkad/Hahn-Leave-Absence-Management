using AutoMapper;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;

namespace HahnLeaveAbsenceManagement.Application.LeaveRequest;

public class LeaveRequestMapper : Profile
{
    public LeaveRequestMapper()
    {
        CreateMap<Domain.LeaveRequest.LeaveRequest, LeaveRequestDto>();
    }
}