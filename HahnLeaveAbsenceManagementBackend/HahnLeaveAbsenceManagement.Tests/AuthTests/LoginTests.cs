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

namespace HahnLeaveAbsenceManagement.Tests.AuthTests;

public class LoginTests
{
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AuthController _controller;

        public LoginTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AuthController();
            var httpCtx = new DefaultHttpContext();
            var services = new ServiceCollection();
            services.AddSingleton(_mediatorMock.Object);
            httpCtx.RequestServices = services.BuildServiceProvider();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpCtx };
        }

        [Fact]
        public async Task Login_ReturnsOk_And_Response_OnSuccess()
        {
            // Arrange
            var cmd = new LoginCommand("a@b.com", "P@ssword1");
            var expected = new LoginResponse
            {
                Id = Guid.NewGuid(),
                Email = "a@b.com",
                Role = UserRole.Employee,
                Token = "jwt-token",
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _controller.Login(cmd);

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Login_Throws_BadRequestException_OnInvalidCredentials()
        {
            // Arrange
            var cmd = new LoginCommand("nope@b.com", "wrong");
            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BadRequestException("Invalid credentials"));

            // Act + Assert (exception bubbles to middleware)
            var act = async () => await _controller.Login(cmd);
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Invalid credentials");
        }

        [Fact]
        public async Task Login_Throws_ValidationException_WhenModelInvalid()
        {
            // Arrange
            var cmd = new LoginCommand("", "123"); // invalid by validator
            var valEx = new FluentValidation.ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Email", "Email is required."),
                new FluentValidation.Results.ValidationFailure("Password", "Password must be at least 8 characters.")
            });

            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(valEx);

            // Act + Assert
            var act = async () => await _controller.Login(cmd);
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Login_Throws_UnauthorizedAccessException_WhenAuthFailsByPolicy()
        {
            // Arrange
            var cmd = new LoginCommand("a@b.com", "P@ssword1");
            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act + Assert
            var act = async () => await _controller.Login(cmd);
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Unauthorized");
        }

        [Fact]
        public async Task Login_Throws_ForbiddenException_WhenForbidden()
        {
            // Arrange
            var cmd = new LoginCommand("a@b.com", "P@ssword1");
            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ForbiddenException());

            // Act + Assert
            var act = async () => await _controller.Login(cmd);
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Login_Throws_InternalServerException_OnUnexpected()
        {
            // Arrange
            var cmd = new LoginCommand("a@b.com", "P@ssword1");
            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InternalServerException());

            // Act + Assert
            var act = async () => await _controller.Login(cmd);
            await act.Should().ThrowAsync<InternalServerException>("Internal server error")
                .WithMessage("Internal server error");
        }
    
    
}