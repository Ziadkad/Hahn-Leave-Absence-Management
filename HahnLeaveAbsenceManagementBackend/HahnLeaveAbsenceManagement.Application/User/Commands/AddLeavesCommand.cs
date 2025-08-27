using AutoMapper;
using FluentValidation;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Application.User.Models;
using MediatR;

namespace HahnLeaveAbsenceManagement.Application.User.Commands;

public record AddLeavesCommand(Guid UserId, int Days) : IRequest<UserDto>;

public class AddLeavesCommandValidator : AbstractValidator<AddLeavesCommand>
{
    public AddLeavesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId should not be empty");
        RuleFor(x => x.Days)
            .NotEmpty().WithMessage("Days should not be empty")
            .GreaterThan(0).WithMessage("Days must be greater than zero");
    }
}

public class AddLeavesCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<AddLeavesCommand, UserDto>
{
    public async Task<UserDto> Handle(AddLeavesCommand request, CancellationToken cancellationToken)
    {
        Domain.User.User? user = await userRepository.FindAsync(request.UserId,cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(user), request.UserId);
        }
         user.AddLeaveDays(request.Days);
         var isSaved = await unitOfWork.SaveChangesAsync(cancellationToken);
         if (isSaved <= 0)
         {
             throw new InternalServerException();
         }
        
         return mapper.Map<UserDto>(user);
    }
}
