using AutoMapper;
using HahnLeaveAbsenceManagement.Application.User.Models;

namespace HahnLeaveAbsenceManagement.Application.User;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<Domain.User.User, LoginResponse>();
    }
}