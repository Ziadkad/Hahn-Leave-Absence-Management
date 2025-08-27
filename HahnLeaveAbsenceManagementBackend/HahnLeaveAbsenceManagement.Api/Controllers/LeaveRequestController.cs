using HahnLeaveAbsenceManagement.Application.LeaveRequest.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HahnLeaveAbsenceManagement.Api.Controllers;

public class LeaveRequestController : BaseController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateLeaveRequestCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> CreateProject(Guid id,[FromBody] UpdateLeaveRequestStatusCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}