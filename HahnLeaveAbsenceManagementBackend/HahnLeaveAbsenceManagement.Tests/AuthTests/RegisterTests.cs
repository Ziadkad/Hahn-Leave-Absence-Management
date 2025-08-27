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

public class RegisterTests
{
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AuthController _controller;

        public RegisterTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AuthController();

            var httpCtx = new DefaultHttpContext();
            var services = new ServiceCollection();
            services.AddSingleton(_mediatorMock.Object);
            httpCtx.RequestServices = services.BuildServiceProvider();
            _controller.ControllerContext = new ControllerContext() { HttpContext = httpCtx };
        }

        [Fact]
        public async Task Register_ReturnsOk_With_LoginResponse_OnSuccess()
        {
            // Arrange
            var cmd = new RegisterCommand("john@site.com", "P@ssw0rd!", "John", "Doe", Domain.User.UserRole.Employee);
            var expected = new LoginResponse()
            {
                Id = Guid.NewGuid(),
                Email = "john@site.com",
                Role = UserRole.Employee,
                Token = "jwt-here",
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _controller.Register(cmd);

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Register_Throws_DuplicateException_WhenEmailAlreadyExists()
        {
            // Arrange
            var email = "exists@site.com";
            var cmd = new RegisterCommand(email, "P@ssw0rd!", "Alice", "Smith", Domain.User.UserRole.Employee);

            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DuplicateException(cmd.Email));

            // Act + Assert (bubbles to middleware)
            var act = async () => await _controller.Register(cmd);
            await act.Should().ThrowAsync<DuplicateException>()
                .WithMessage($"the entity '{email}' already exists.");
        }

        [Fact]
        public async Task Register_Throws_ValidationException_WhenPayloadInvalid()
        {
            // Arrange: clearly invalid (empty email + short password)
            var cmd = new RegisterCommand("", "123", "", "", Domain.User.UserRole.Employee);

            var valEx = new FluentValidation.ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Email","Email is required."),
                new FluentValidation.Results.ValidationFailure("Password","Password must be at least 8 characters."),
                new FluentValidation.Results.ValidationFailure("FirstName","First name is required."),
                new FluentValidation.Results.ValidationFailure("LastName","Last name is required.")
            });

            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(valEx);

            // Act + Assert
            var act = async () => await _controller.Register(cmd);
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task Register_Throws_InternalServerException_WhenSavingFails()
        {
            // Arrange
            var cmd = new RegisterCommand("new@site.com", "P@ssw0rd!", "New", "User", Domain.User.UserRole.Employee);

            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InternalServerException());

            // Act + Assert
            var act = async () => await _controller.Register(cmd);
            await act.Should().ThrowAsync<InternalServerException>()
                .WithMessage("Internal server error");
        }

        [Fact]
        public async Task Register_Throws_UnauthorizedAccessException_WhenUnauthorized()
        {
            // Arrange
            var cmd = new RegisterCommand("john@site.com", "P@ssw0rd!", "John", "Doe", Domain.User.UserRole.Employee);

            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act + Assert
            var act = async () => await _controller.Register(cmd);
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Unauthorized");
        }

        [Fact]
        public async Task Register_Throws_ForbiddenException_WhenForbidden()
        {
            // Arrange
            var cmd = new RegisterCommand("john@site.com", "P@ssw0rd!", "John", "Doe", Domain.User.UserRole.Employee);

            _mediatorMock
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ForbiddenException());

            // Act + Assert
            var act = async () => await _controller.Register(cmd);
            await act.Should().ThrowAsync<ForbiddenException>();
        }
}