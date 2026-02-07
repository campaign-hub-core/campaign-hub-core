using CampaignHub.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace CampaignHub.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra os serviços da camada de aplicação (use cases, application services).
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
