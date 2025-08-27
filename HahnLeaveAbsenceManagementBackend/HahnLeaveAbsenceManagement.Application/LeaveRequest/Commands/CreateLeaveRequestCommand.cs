using System.Windows.Input;
using AutoMapper;
using FluentValidation;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Extensions;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using MediatR;

namespace HahnLeaveAbsenceManagement.Application.LeaveRequest.Commands;

public record CreateLeaveRequestCommand(LeaveType Type, DateTime StartDate, DateTime EndDate, string Description):IRequest<LeaveRequestDto>;

public class CreateLeaveRequestCommandValidator : AbstractValidator<CreateLeaveRequestCommand>
{
    public CreateLeaveRequestCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid leave type.");

        RuleFor(x => x.StartDate)
            .NotEqual(default(DateTime))
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEqual(default(DateTime))
            .WithMessage("End date is required.");

        RuleFor(x => x)
            .Must(x => x.EndDate.Date >= x.StartDate.Date)
            .WithMessage("End date must be on or after the start date.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");
    }
}

public class CreateLeaveRequestCommandHandler(IUserRepository userRepository, ILeaveRequestRepository leaveRequestRepository, IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper): IRequestHandler<CreateLeaveRequestCommand, LeaveRequestDto>
{
    public async Task<LeaveRequestDto> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        Guid userId = userContext.GetCurrentUserId();
        
        Domain.User.User? user = await userRepository.FindAsync(userId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User", userId);
        }
        
        if (await leaveRequestRepository.HasOverlappingLeaveAsync(userId, request.StartDate, request.EndDate, cancellationToken))
        {
            throw new BadRequestException("You already have an active leave request in this period.");
        }
        
        int businessDays = DateExtensions.GetBusinessDays(request.StartDate, request.EndDate);

        if (user.LeavesLeft < businessDays)
        {
            throw new BadRequestException(
                $"Insufficient leave balance. You have {user.LeavesLeft} day(s) remaining, but this request requires {businessDays} day(s)."
            );
        }

        user.RemoveLeaveDays(businessDays);
        
        Domain.LeaveRequest.LeaveRequest leaveRequest = new Domain.LeaveRequest.LeaveRequest(request.Type, request.StartDate, request.EndDate, request.Description, businessDays, userId);
        
        await leaveRequestRepository.AddAsync(leaveRequest, cancellationToken);

        var isSaved = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (isSaved <= 0)
        {
            throw new InternalServerException();
        }
        
        return mapper.Map<LeaveRequestDto>(leaveRequest);
    }
}