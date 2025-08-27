using HahnLeaveAbsenceManagement.Api.Security;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.User.Commands;
using HahnLeaveAbsenceManagement.Application.User.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HahnLeaveAbsenceManagement.Api.Controllers;

public class UserController : BaseController
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await Mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }
    
    [Authorize(AuthPolicyName.HumanResourcesManager)]
    [HttpPut("AddLeaves/{userId}")]
    public async Task<IActionResult> AddLeaves(Guid userId, AddLeavesCommand command)
    {
        if (userId != command.UserId)
        {
            throw new BadRequestException("Ids do not match");
        }
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}