using FluentAssertions;
using HahnLeaveAbsenceManagement.Api.Controllers;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Extensions;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Commands;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HahnLeaveAbsenceManagement.Tests.LeaveRequestTests;

public class CreateLeaveRequestTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly LeaveRequestController _controller;

    public CreateLeaveRequestTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new LeaveRequestController(); // inherits BaseController
        var httpCtx = new DefaultHttpContext();
        var services = new ServiceCollection();
        services.AddSingleton(_mediatorMock.Object);
        httpCtx.RequestServices = services.BuildServiceProvider();
        _controller.ControllerContext = new ControllerContext() { HttpContext = httpCtx };
    }

    [Fact]
    public async Task CreateLeaveRequest_ReturnsOk_WithDto_OnSuccess()
    {
        // Arrange
        DateTime startDate = DateTimeOffset.Now.Date;
        DateTime endDate = DateTimeOffset.Now.Date.AddDays(4);
        
        var cmd = new CreateLeaveRequestCommand(
            Domain.LeaveRequest.LeaveType.Vacation,
            startDate,
            endDate,
            "Family event"
        );
        int businessDays = DateExtensions.GetBusinessDays(startDate, endDate);
        var expected = new LeaveRequestDto()
        {
            // Fill with whatever your DTO exposes; we assert equivalence below
            Id = Guid.NewGuid(),
            Type = LeaveType.Vacation,
            StartDate = cmd.StartDate,
            EndDate = cmd.EndDate,
            Description = cmd.Description,
            BusinessDays = businessDays,
            Status = LeaveStatus.Pending,
        };

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.CreateLeaveRequest(cmd);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateLeaveRequest_Throws_ValidationException_WhenPayloadInvalid()
    {
        // Arrange: invalid (EndDate < StartDate) or other rule
        var cmd = new CreateLeaveRequestCommand(
            Domain.LeaveRequest.LeaveType.Sick,
            new DateTime(2025, 08, 27),
            new DateTime(2025, 08, 26),
            "oops"
        );

        var valEx = new FluentValidation.ValidationException(new[]
        {
            new FluentValidation.Results.ValidationFailure("", "End date must be on or after the start date.")
        });

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(valEx);

        // Act + Assert
        var act = async () => await _controller.CreateLeaveRequest(cmd);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task CreateLeaveRequest_Throws_NotFound_WhenUserMissing()
    {
        // Arrange
        var cmd = new CreateLeaveRequestCommand(
            Domain.LeaveRequest.LeaveType.Sick,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            "Flu"
        );
        Guid id = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User", id));

        // Act + Assert
        var act = async () => await _controller.CreateLeaveRequest(cmd);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"(User) with key ({id}) was not found");
    }

    [Fact]
    public async Task CreateLeaveRequest_Throws_BadRequest_WhenOverlappingLeave()
    {
        // Arrange
        var cmd = new CreateLeaveRequestCommand(
            Domain.LeaveRequest.LeaveType.Vacation,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(2),
            "Overlap"
        );

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException("You already have an active leave request in this period."));

        // Act + Assert
        var act = async () => await _controller.CreateLeaveRequest(cmd);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("You already have an active leave request in this period.");
    }

    [Fact]
    public async Task CreateLeaveRequest_Throws_BadRequest_WhenInsufficientBalance()
    {
        // Arrange
        var cmd = new CreateLeaveRequestCommand(
            Domain.LeaveRequest.LeaveType.Vacation,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(10),
            "Long trip"
        );

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException(
                "Insufficient leave balance. You have 2 day(s) remaining, but this request requires 7 day(s)."));

        // Act + Assert
        var act = async () => await _controller.CreateLeaveRequest(cmd);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Insufficient leave balance*");
    }

    [Fact]
    public async Task CreateLeaveRequest_Throws_InternalServerException_OnSaveFailure()
    {
        // Arrange
        var cmd = new CreateLeaveRequestCommand(
            Domain.LeaveRequest.LeaveType.Vacation,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(1),
            "DB fail sim"
        );

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException());

        // Act + Assert
        var act = async () => await _controller.CreateLeaveRequest(cmd);
        await act.Should().ThrowAsync<InternalServerException>()
            .WithMessage("Internal server error");
    }

    [Fact]
    public async Task CreateLeaveRequest_Throws_UnauthorizedAccess_WhenUnauthorized()
    {
        // Arrange
        var cmd = new CreateLeaveRequestCommand(
            Domain.LeaveRequest.LeaveType.Sick,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            "Auth issue"
        );

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

        // Act + Assert
        var act = async () => await _controller.CreateLeaveRequest(cmd);
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Unauthorized");
    }
}