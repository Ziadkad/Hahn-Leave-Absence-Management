using AutoMapper;
using FluentValidation;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Application.LeaveRequest.Models;
using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using HahnLeaveAbsenceManagement.Domain.User;
using MediatR;

namespace HahnLeaveAbsenceManagement.Application.LeaveRequest.Commands;

public record UpdateLeaveRequestStatusCommand(Guid Id, LeaveStatus Status): IRequest<LeaveRequestDto>;

public class UpdateLeaveRequestStatusCommandValidator: AbstractValidator<UpdateLeaveRequestStatusCommand>
{
    public UpdateLeaveRequestStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid leave type.");
    }
}
public class UpdateLeaveRequestStatusCommandHandler(ILeaveRequestRepository leaveRequestRepository, IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper) : IRequestHandler<UpdateLeaveRequestStatusCommand, LeaveRequestDto>
{
    public async Task<LeaveRequestDto> Handle(UpdateLeaveRequestStatusCommand request, CancellationToken cancellationToken)
    {
        Domain.LeaveRequest.LeaveRequest? leaveRequest= await leaveRequestRepository.FindAsync(request.Id, cancellationToken);
        if (leaveRequest == null)
        {
            throw new NotFoundException(nameof(LeaveRequest), request.Id);
        }
        
        if (userContext.GetUserRole() == UserRole.Employee)
        {
            if (leaveRequest.UserId != userContext.GetCurrentUserId())
            {
                throw new ForbiddenException();
            }
            if (request.Status != LeaveStatus.Cancelled)
            {
                throw new ForbiddenException();
            }

            if (leaveRequest.Status != LeaveStatus.Pending)
            {
                throw new BadRequestException("Only leave requests with status 'Pending' can be cancelled.");
            }
        }
        
        else if (userContext.GetUserRole() == UserRole.HumanResourcesManager && request.Status == LeaveStatus.Pending)
        { 
            throw new BadRequestException(
                "Invalid status change for HR: 'Pending' are not allowed targets." 
            );
        }
        
        leaveRequest.UpdateStatus(request.Status);
        
        var isSaved = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (isSaved <= 0)
        {
            throw new InternalServerException();
        }
        
        return mapper.Map<LeaveRequestDto>(leaveRequest);
    }
}