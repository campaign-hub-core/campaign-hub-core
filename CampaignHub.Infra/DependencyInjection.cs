using CampaignHub.Application.Interfaces;
using CampaignHub.Application.Options;
using CampaignHub.Domain.Interfaces;
using CampaignHub.Infra.ExternalServices.Meta;
using CampaignHub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace CampaignHub.Infra;

public static class DependencyInjection
{
    /// <summary>
    /// Registra os serviços da camada de infraestrutura (DbContext, Unit of Work, Repositórios, HttpClients).
    /// </summary>
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        var connectionString = configuration.GetConnectionString("PostegreSqlConnection");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAdAccountRepository, AdAccountRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<IMetricCampaignRepository, MetricRepository>();
        services.AddScoped<IAdSetRepository, AdSetRepository>();
        services.AddScoped<IAdRepository, AdRepository>();

        // Meta Ads Options
        services.Configure<MetaAdsOptions>(configuration.GetSection(MetaAdsOptions.SectionName));

        // Meta Ads HttpClient
        services.AddTransient<MetaTokenDelegatingHandler>();

        var metaAdsSection = configuration.GetSection(MetaAdsOptions.SectionName);
        var timeoutSeconds = metaAdsSection.GetValue<int?>("TimeoutSeconds") ?? 30;

        services.AddHttpClient<IMetaAdsApiClient, MetaAdsApiClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        })
        .AddHttpMessageHandler<MetaTokenDelegatingHandler>()
        .AddStandardResilienceHandler();

        return services;
    }
}
