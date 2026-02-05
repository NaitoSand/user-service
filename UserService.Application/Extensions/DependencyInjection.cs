namespace UserService.Application.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Services;

/// <summary>
/// Registers Application-layer dependencies such as domain services, validators, etc.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}