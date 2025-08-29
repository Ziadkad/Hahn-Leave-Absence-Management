using FluentAssertions;
using HahnLeaveAbsenceManagement.Api.Controllers;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.User.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
    
namespace HahnLeaveAbsenceManagement.Tests.UserTests;

public class GetLeavesLeftTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UserController _controller;

    public GetLeavesLeftTests()
    {
        _mediatorMock = new Mock<IMediator>();

        _controller = new UserController(); // assumes controller resolves Mediator from HttpContext

        var httpCtx = new DefaultHttpContext();
        var services = new ServiceCollection();
        services.AddSingleton<IMediator>(_mediatorMock.Object);
        httpCtx.RequestServices = services.BuildServiceProvider();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpCtx };
    }

    [Fact]
    public async Task GetLeavesLeft_ReturnsOk_WithInt_OnSuccess()
    {
        // Arrange
        const int expected = 13;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetLeavesLeftQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetLeavesLeft();

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().Be(expected);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetLeavesLeftQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLeavesLeft_Throws_NotFound_WhenUserMissing()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetLeavesLeftQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User", id));

        // Act
        var act = async () => await _controller.GetLeavesLeft();

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"(User) with key ({id}) was not found");
    }

    [Fact]
    public async Task GetLeavesLeft_Throws_Unauthorized_WhenNotAuthenticated()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetLeavesLeftQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

        // Act
        var act = async () => await _controller.GetLeavesLeft();

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Unauthorized");
    }
}
