using System.Net;
using System.Text.Json;
using CampaignHub.Application.Exceptions;

namespace CampaignHub.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Entidade não encontrada: {Entity} com id {Id}", ex.EntityName, ex.Id);
            await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (MetaApiException ex)
        {
            _logger.LogError(ex, "Erro na API do Meta Ads: Code={ErrorCode}, Subcode={ErrorSubcode}, Message={Message}",
                ex.ErrorCode, ex.ErrorSubcode, ex.Message);
            await WriteResponseAsync(context, ex.HttpStatusCode, ex.Message, ex.ErrorCode, ex.ErrorSubcode);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de regra de negócio: {Message}", ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso não encontrado: {Message}", ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno não tratado: {Message}", ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message,
        int? errorCode = null, int? errorSubcode = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        object response = errorCode.HasValue
            ? new
            {
                status = (int)statusCode,
                message,
                errorCode,
                errorSubcode
            }
            : new
            {
                status = (int)statusCode,
                message
            };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
