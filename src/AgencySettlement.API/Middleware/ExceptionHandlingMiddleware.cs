using AgencySettlement.Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AgencySettlement.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug("Request cancelled. TraceId: {TraceId}", context.TraceIdentifier);
            if (!context.Response.HasStarted)
                context.Response.StatusCode = 499;
        }
        catch (ExternalServiceException ex)
        {
            logger.LogError(ex, "External service failure. Code: {Code}, TraceId: {TraceId}", ex.Code, context.TraceIdentifier);
            await WriteProblemAsync(context, ex.StatusCode == 504 ? 504 : 502, "External service error", "دریافت اطلاعات از سرویس خارجی با مشکل مواجه شد.", ex.Code);
        }
        catch (BusinessException ex)
        {
            var statusCode = ex.Code switch
            {
                "IDEMPOTENCY_KEY_REUSED" or "IDEMPOTENCY_CONFLICT" => StatusCodes.Status409Conflict,
                "AGENCY_NOT_FOUND" or "SETTLEMENT_NOT_FOUND" => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status422UnprocessableEntity
            };

            logger.LogWarning(ex, "Business rule failed. Code: {Code}, TraceId: {TraceId}", ex.Code, context.TraceIdentifier);
            await WriteProblemAsync(context, statusCode, "Business rule violation", ex.Message, ex.Code);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Resource not found", ex.Message, "RESOURCE_NOT_FOUND");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Internal server error", "خطای غیرمنتظره‌ای در پردازش درخواست رخ داد.", "INTERNAL_SERVER_ERROR");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail, string code)
    {
        if (context.Response.HasStarted) return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}"
        };
        problem.Extensions["code"] = code;
        problem.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
    }
}
