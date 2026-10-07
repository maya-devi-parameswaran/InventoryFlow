using InventoryFlow.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using ApplicationValidationException = InventoryFlow.Application.Common.Exceptions.ValidationException;

namespace InventoryFlow.Api.Middleware;

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Explicitly specify the tuple type to resolve deconstruction and type inference errors
        (HttpStatusCode statusCode, object response) result = exception switch
        {
            ApplicationValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new { title = "Validation failed", errors = validationEx.Errors }
            ),

            InsufficientStockException stockEx => (
                HttpStatusCode.Conflict,
                new { title = "Insufficient stock", detail = stockEx.Message, errors = (object?)null }
            ),

            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                new { title = "Business rule violation", detail = domainEx.Message, errors = (object?)null }
            ),

            UnauthorizedAccessException => (
                HttpStatusCode.Forbidden,
                new { title = "Forbidden", detail = "You do not have permission to perform this action.", errors = (object?)null }
            ),

            _ => (
                HttpStatusCode.InternalServerError,
                new { title = "An unexpected error occurred.", detail = (string?)null, errors = (object?)null }
            )
        };

        var (statusCode, response) = result;

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception occurred");
        }

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}