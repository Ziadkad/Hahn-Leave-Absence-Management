using FluentAssertions;
using HahnLeaveAbsenceManagement.Api.Controllers;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Commands;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HahnLeaveAbsenceManagement.Tests.LeaveRequestTests;

public class UpdateLeaveRequestStatusTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly LeaveRequestController _controller;

    public UpdateLeaveRequestStatusTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new LeaveRequestController();
        var httpCtx = new DefaultHttpContext();
        var services = new ServiceCollection();
        services.AddSingleton(_mediatorMock.Object);
        httpCtx.RequestServices = services.BuildServiceProvider();
        _controller.ControllerContext = new ControllerContext() { HttpContext = httpCtx };
    }

    [Fact]
    public async Task UpdateStatus_ReturnsOk_WithDto_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cmd = new UpdateLeaveRequestStatusCommand(id, LeaveStatus.Approved);
        var expected = new LeaveRequestDto()
        {
            Id = id,
            Type = LeaveType.Vacation,
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(2),
            Description = "ok",
            BusinessDays = 2,
            Status = LeaveStatus.Approved,
        };

        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

        // Act
        var result = await _controller.UpdateLeaveRequestStatus(id, cmd);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateStatus_Throws_BadRequest_WhenRouteAndBodyIdsMismatch()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var bodyId  = Guid.NewGuid(); // different
        var cmd = new UpdateLeaveRequestStatusCommand(bodyId, LeaveStatus.Cancelled);

        // Act + Assert (controller guard triggers; mediator not called)
        var act = async () => await _controller.UpdateLeaveRequestStatus(routeId, cmd);
        await act.Should().ThrowAsync<BadRequestException>()
                 .WithMessage("Ids do not match");

        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateLeaveRequestStatusCommand>(), It.IsAny<CancellationToken>()),
                             Times.Never);
    }

    [Fact]
    public async Task UpdateStatus_Throws_NotFound_WhenLeaveRequestMissing()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateLeaveRequestStatusCommand(id, LeaveStatus.Approved);

        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new NotFoundException(nameof(LeaveRequest), id));

        var act = async () => await _controller.UpdateLeaveRequestStatus(id, cmd);
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage($"({nameof(LeaveRequest)}) with key ({id}) was not found");
    }

    [Fact]
    public async Task UpdateStatus_Throws_Forbidden_WhenPolicyDenies()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateLeaveRequestStatusCommand(id, LeaveStatus.Rejected);

        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new ForbiddenException());

        var act = async () => await _controller.UpdateLeaveRequestStatus(id, cmd);
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task UpdateStatus_Throws_BadRequest_WhenEmployeeCancelsNonPending()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateLeaveRequestStatusCommand(id, LeaveStatus.Cancelled);

        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new BadRequestException("Only leave requests with status 'Pending' can be cancelled."));

        var act = async () => await _controller.UpdateLeaveRequestStatus(id, cmd);
        await act.Should().ThrowAsync<BadRequestException>()
                 .WithMessage("Only leave requests with status 'Pending' can be cancelled.");
    }

    [Fact]
    public async Task UpdateStatus_Throws_BadRequest_WhenHRTargetsPending()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateLeaveRequestStatusCommand(id, LeaveStatus.Pending);

        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new BadRequestException("Invalid status change for HR: 'Pending' are not allowed targets."));

        var act = async () => await _controller.UpdateLeaveRequestStatus(id, cmd);
        await act.Should().ThrowAsync<BadRequestException>()
                 .WithMessage("Invalid status change for HR: 'Pending' are not allowed targets.");
    }

    [Fact]
    public async Task UpdateStatus_Throws_Unauthorized_WhenUnauthorized()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateLeaveRequestStatusCommand(id, LeaveStatus.Approved);

        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

        var act = async () => await _controller.UpdateLeaveRequestStatus(id, cmd);
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
                 .WithMessage("Unauthorized");
    }

    [Fact]
    public async Task UpdateStatus_Throws_InternalServerException_OnSaveFailure()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateLeaveRequestStatusCommand(id, LeaveStatus.Approved);

        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new InternalServerException());

        var act = async () => await _controller.UpdateLeaveRequestStatus(id, cmd);
        await act.Should().ThrowAsync<InternalServerException>()
                 .WithMessage("Internal server error");
    }

    [Fact]
    public async Task UpdateStatus_Throws_ValidationException_WhenStatusInvalid()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateLeaveRequestStatusCommand(id, (LeaveStatus)999); // invalid enum

        var valEx = new FluentValidation.ValidationException(new[]
        {
            new FluentValidation.Results.ValidationFailure("Status","Invalid leave type.")
        });

        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(valEx);

        var act = async () => await _controller.UpdateLeaveRequestStatus(id, cmd);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}