using CampaignHub.API.Middlewares;

namespace CampaignHub.API.Extensions;

public static class ApiApplicationBuilderExtensions
{
    /// <summary>
    /// Configura o pipeline HTTP da API (Swagger, redirecionamento, autorização, rotas).
    /// </summary>
    public static WebApplication UseCampaignHubApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "CampaignHub API v1");
        });

        app.UseCors("AllowAll");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
