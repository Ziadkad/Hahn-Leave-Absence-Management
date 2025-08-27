using FluentAssertions;
using HahnLeaveAbsenceManagement.Api.Controllers;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.User.Models;
using HahnLeaveAbsenceManagement.Application.User.Queries;
using HahnLeaveAbsenceManagement.Domain.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HahnLeaveAbsenceManagement.Tests.UserTests;

public class GetAllUsersTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UserController _controller;

    public GetAllUsersTests()
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
    public async Task GetAllUsers_ReturnsOk_WithUsers_OnSuccess()
    {
        // Arrange
        var expected = new List<UserDto>
        {
            new UserDto { Id = Guid.NewGuid(), FirstName = "A", LastName = "One", Role = UserRole.Employee, LeavesLeft = 10},
            new UserDto { Id = Guid.NewGuid(), FirstName = "B", LastName = "Two", Role =  UserRole.HumanResourcesManager, LeavesLeft = 18}
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOk_WithEmptyList_WhenNoUsers()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserDto>());

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        (ok.Value as IEnumerable<UserDto>).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllUsers_Throws_UnauthorizedAccessException_WhenUnauthorized()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

        // Act + Assert (bubbles to middleware)
        var act = async () => await _controller.GetAllUsers();
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Unauthorized");
    }
    

    [Fact]
    public async Task GetAllUsers_Throws_InternalServerException_OnUnexpectedFailure()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException());

        // Act + Assert
        var act = async () => await _controller.GetAllUsers();
        await act.Should().ThrowAsync<InternalServerException>()
            .WithMessage("Internal server error");
    }
}