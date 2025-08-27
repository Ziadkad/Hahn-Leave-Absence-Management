using FluentAssertions;
using HahnLeaveAbsenceManagement.Api.Controllers;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.User.Commands;
using HahnLeaveAbsenceManagement.Application.User.Models;
using HahnLeaveAbsenceManagement.Domain.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HahnLeaveAbsenceManagement.Tests.UserTests;

public class AddLeavesTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UserController _controller;

    public AddLeavesTests()
    {
        _mediatorMock = new Mock<IMediator>();

        _controller = new UserController();
        var httpCtx = new DefaultHttpContext();
        var services = new ServiceCollection();
        services.AddSingleton(_mediatorMock.Object);
        httpCtx.RequestServices = services.BuildServiceProvider();
        _controller.ControllerContext = new ControllerContext() { HttpContext = httpCtx };
    }

    [Fact]
    public async Task AddLeaves_ReturnsOk_WithUserDto_OnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = new AddLeavesCommand(userId, 5);
        var expected = new UserDto()
        {
            Id = userId,
            FirstName = "User",
            LastName = "One",
            Role = UserRole.Employee,
            LeavesLeft = 15
        };

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.AddLeaves(userId, cmd);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task AddLeaves_Throws_BadRequest_WhenRouteAndBodyIdsMismatch()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var bodyId = Guid.NewGuid(); // different
        var cmd = new AddLeavesCommand(bodyId, 3);

        // Act + Assert (controller throws before mediator is called)
        var act = async () => await _controller.AddLeaves(routeId, cmd);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Ids do not match");

        // Ensure mediator was never called
        _mediatorMock.Verify(m => m.Send(It.IsAny<AddLeavesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddLeaves_Throws_NotFound_WhenUserMissing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = new AddLeavesCommand(userId, 2);

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("user", userId));

        // Act + Assert
        var act = async () => await _controller.AddLeaves(userId, cmd);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"({nameof(User)}) with key ({userId}) was not found");
    }

    [Fact]
    public async Task AddLeaves_Throws_ValidationException_WhenInvalidPayload()
    {
        // Arrange: invalid days (<= 0)
        var userId = Guid.NewGuid();
        var cmd = new AddLeavesCommand(userId, 0);

        var valEx = new FluentValidation.ValidationException(new[]
        {
            new FluentValidation.Results.ValidationFailure("Days","Days must be greater than zero")
        });

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(valEx);

        // Act + Assert
        var act = async () => await _controller.AddLeaves(userId, cmd);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task AddLeaves_Throws_InternalServerException_WhenSaveFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = new AddLeavesCommand(userId, 4);

        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException());

        // Act + Assert
        var act = async () => await _controller.AddLeaves(userId, cmd);
        await act.Should().ThrowAsync<InternalServerException>()
            .WithMessage("Internal server error");
    }

    [Fact]
    public async Task AddLeaves_Throws_ForbiddenException_WhenPolicyDenies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = new AddLeavesCommand(userId, 1);

        // In real pipeline, [Authorize(...)] would block; here we simulate via mediator
        _mediatorMock
            .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ForbiddenException());

        // Act + Assert
        var act = async () => await _controller.AddLeaves(userId, cmd);
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("The current user is not allowed to perform this action");
    }
}