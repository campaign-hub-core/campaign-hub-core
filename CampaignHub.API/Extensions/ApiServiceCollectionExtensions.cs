using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CampaignHub.API.Extensions;

public static class ApiServiceCollectionExtensions
{
    /// <summary>
    /// Registra os serviços específicos da API (Controllers, Swagger).
    /// </summary>
    public static IServiceCollection AddCampaignHubApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CampaignHub API",
                Version = "v1"
            });
        });

        return services;
    }
}
