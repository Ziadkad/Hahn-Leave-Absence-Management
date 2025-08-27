using FluentAssertions;
using HahnLeaveAbsenceManagement.Api.Controllers;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Models;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Queries;
using HahnLeaveAbsenceManagement.Application.User.Models;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HahnLeaveAbsenceManagement.Tests.LeaveRequestTests;

public class GetAllLeaveRequestsTests
{
    private readonly Mock<IMediator> _mediatorMock;
        private readonly LeaveRequestController _controller;

        public GetAllLeaveRequestsTests()
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
        public async Task GetAll_ReturnsOk_WithPagedResult_OnSuccess()
        {
            // Arrange filters
            var userId        = Guid.NewGuid();
            var type          = LeaveType.Vacation;
            var startDateFrom = new DateTime(2025, 8, 1);
            var startDateTo   = new DateTime(2025, 8, 31);
            var endDateFrom   = new DateTime(2025, 8, 2);
            var endDateTo     = new DateTime(2025, 9, 1);
            var status        = LeaveStatus.Approved;
            var pageQuery     = new PageQueryRequest()
            {
                Page = 1,
                PageSize = 10,
            }; 

            var expected = new PageQueryResult<LeaveRequestWithUserDto>(
                new List<LeaveRequestWithUserDto>
                {
                    new LeaveRequestWithUserDto
                    {
                        Id = Guid.NewGuid(),
                        Type = type,
                        StartDate = new DateTime(2025, 8, 10),
                        EndDate = new DateTime(2025, 8, 12),
                        BusinessDays = 2,
                        Status = status,
                        Description = "Approved leave",
                        User = new UserDto()
                    }
                },
                1
            );

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<GetAllLeaveRequestQuery>(q =>
                        q.UserId == userId &&
                        q.Type == type &&
                        q.StartDateFrom == startDateFrom &&
                        q.StartDateTo == startDateTo &&
                        q.EndDateFrom == endDateFrom &&
                        q.EndDateTo == endDateTo &&
                        q.Status == status &&
                        ReferenceEquals(q.PageQuery, pageQuery)
                    ),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _controller.GetAll(
                userId, type, startDateFrom, startDateTo, endDateFrom, endDateTo, status, pageQuery);

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllLeaveRequestQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithEmptyPage_WhenNoData()
        {
            // Arrange
            var pageQuery = new PageQueryRequest();
            var expected = new PageQueryResult<LeaveRequestWithUserDto>(new List<LeaveRequestWithUserDto>(), 0);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllLeaveRequestQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _controller.GetAll(null, null, null, null, null, null, null, pageQuery);

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAll_Throws_BadRequest_OnInvalidFilterCombination()
        {
            // Arrange (simulate repo/handler rejecting invalid range, etc.)
            var pageQuery = new PageQueryRequest();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllLeaveRequestQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BadRequestException("Invalid date range"));

            // Act + Assert
            var act = async () => await _controller.GetAll(
                null, LeaveType.Sick,
                new DateTime(2025, 8, 31), // startFrom > startTo to simulate invalid
                new DateTime(2025, 8, 1),
                null, null, null, pageQuery);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Invalid date range");
        }

        [Fact]
        public async Task GetAll_Throws_Unauthorized_WhenNotAuthenticated()
        {
            var pageQuery = new PageQueryRequest();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllLeaveRequestQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            var act = async () => await _controller.GetAll(null, null, null, null, null, null, null, pageQuery);
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Unauthorized");
        }

        [Fact]
        public async Task GetAll_Throws_Forbidden_WhenPolicyBlocks()
        {
            var pageQuery = new PageQueryRequest();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllLeaveRequestQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ForbiddenException());

            var act = async () => await _controller.GetAll(null, null, null, null, null, null, null, pageQuery);
            await act.Should().ThrowAsync<ForbiddenException>()
                .WithMessage("The current user is not allowed to perform this action");
        }

        [Fact]
        public async Task GetAll_Throws_InternalServer_OnUnexpectedFailure()
        {
            var pageQuery = new PageQueryRequest();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllLeaveRequestQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InternalServerException());

            var act = async () => await _controller.GetAll(null, null, null, null, null, null, null, pageQuery);
            await act.Should().ThrowAsync<InternalServerException>()
                .WithMessage("Internal server error");
        }
}