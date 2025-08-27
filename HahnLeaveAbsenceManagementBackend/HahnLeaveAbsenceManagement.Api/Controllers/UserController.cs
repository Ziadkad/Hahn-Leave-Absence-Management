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

}