using System.Reflection;
using FluentValidation;
using HahnLeaveAbsenceManagement.Application.Common.Helper;
using HahnLeaveAbsenceManagement.Application.Common.Interfaces;
using HahnLeaveAbsenceManagement.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace HahnLeaveAbsenceManagement.Application;

public static class DependencyInjection
{
    public static void RegisterApplicationServices(
        this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(BaseModel))!));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // services.AddAutoMapper(typeof(ResourceMapper).Assembly);

        services.AddScoped<ITokenService, TokenService>();
    }
}