using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Models;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Commands;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Queries;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HahnLeaveAbsenceManagement.Api.Controllers;

public class LeaveRequestController : BaseController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeaveRequestStatus(Guid id,[FromBody] UpdateLeaveRequestStatusCommand command)
    {
        if (id != command.Id)
        {
            throw new BadRequestException("Ids do not match");
        }
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PageQueryResult<LeaveRequestWithUserDto>>> GetAll(
        [FromQuery] Guid? userId,
        [FromQuery] LeaveType? type,
        [FromQuery] DateTime? startDateFrom,
        [FromQuery] DateTime? startDateTo,
        [FromQuery] DateTime? endDateFrom,
        [FromQuery] DateTime? endDateTo,
        [FromQuery] LeaveStatus? status,
        [FromQuery] PageQueryRequest pageQuery)
    {
        var query = new GetAllLeaveRequestQuery(
            userId,
            type,
            startDateFrom,
            startDateTo,
            endDateFrom,
            endDateTo,
            status,
            pageQuery
        );

        var result = await Mediator.Send(query);

        return Ok(result);
    }

    
}