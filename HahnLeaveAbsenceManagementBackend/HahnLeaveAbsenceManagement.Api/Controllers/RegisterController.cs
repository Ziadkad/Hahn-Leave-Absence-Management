using HahnLeaveAbsenceManagement.Application.User.Commands;
using HahnLeaveAbsenceManagement.Application.User.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HahnLeaveAbsenceManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>() ?? throw new InvalidOperationException("Mediator not found in request services.");
    
    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Register([FromForm] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}