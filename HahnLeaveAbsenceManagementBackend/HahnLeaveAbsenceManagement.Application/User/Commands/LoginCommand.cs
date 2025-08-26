using AutoMapper;
using FluentValidation;
using HahnLeaveAbsenceManagement.Application.Common.Exceptions;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Application.User.Models;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace HahnLeaveAbsenceManagement.Application.User.Commands;

public record LoginCommand(string Email, string Password):IRequest<LoginResponse>;


public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
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
    }
}
public class LoginCommandHandler(IUserRepository userRepository,ITokenService tokenService, IMapper mapper,IConfiguration configuration) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Domain.User.User? user = await userRepository.FindByEmailAsync(request.Email,cancellationToken);
        if (user is null)
        {
            throw new BadRequestException("Invalid credentials");
        }
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!isPasswordValid)
        {
            throw new BadRequestException("Invalid credentials");
        }
        var jwtSettings = configuration.GetSection("JwtSettings");
        var expirationMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);
        LoginResponse response = mapper.Map<LoginResponse>(user);
        response.Token = tokenService.GenerateToken(user);
        response.TokenExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
        return response;
    }
}