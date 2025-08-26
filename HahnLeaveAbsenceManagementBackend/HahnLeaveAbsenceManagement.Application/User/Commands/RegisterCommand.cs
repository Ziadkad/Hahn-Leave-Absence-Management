using AutoMapper;
using FluentValidation;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Application.User.Models;
using HahnLeaveAbsenceManagement.Domain.User;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace HahnLeaveAbsenceManagement.Application.User.Commands;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName, UserRole Role) : IRequest<LoginResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Role is invalid.");
    }
}

public class RegisterCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IConfiguration configuration) : IRequestHandler<RegisterCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email,cancellationToken))
            throw new DuplicateException(request.Email);
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        Domain.User.User user = new Domain.User.User(request.Email, passwordHash, request.FirstName, request.LastName, request.Role);
        await userRepository.AddAsync(user,cancellationToken);
        var isSaved = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (isSaved <= 0)
        {
            throw new InternalServerException();
        }
        
        var jwtSettings = configuration.GetSection("JwtSettings");
        var expirationMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);
        LoginResponse response = mapper.Map<LoginResponse>(user);
        response.Token = tokenService.GenerateToken(user);
        response.TokenExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
        return response;
    }
}