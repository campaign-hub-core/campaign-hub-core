using CampaignHub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampaignHub.Infra;

public static class DependencyInjection
{
    /// <summary>
    /// Registra os serviços da camada de infraestrutura (DbContext, Unit of Work, Repositórios).
    /// </summary>
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostegreSqlConnection");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
