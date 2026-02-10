using CampaignHub.Application.AdAccounts;
using CampaignHub.Application.Auth;
using CampaignHub.Application.Campaigns;
using CampaignHub.Application.Customers;
using CampaignHub.Application.Metrics;
using CampaignHub.Application.Organizations;
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
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAdAccountService, AdAccountService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IMetricService, MetricService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
