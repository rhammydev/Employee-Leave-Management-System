using System.Net;
using System.Text.Json;
using EmployeeLeaveManagementSystem.Exceptions;

namespace EmployeeLeaveManagementSystem.Middleware;

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
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,            
            BadRequestException => HttpStatusCode.BadRequest,        
            ConflictException => HttpStatusCode.Conflict,         
            InvalidOperationException => HttpStatusCode.Conflict,    
            _ => HttpStatusCode.InternalServerError                  
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            // Log full details for unexpected errors only — don't leak stack traces to clients.
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }

        var message = statusCode == HttpStatusCode.InternalServerError
            ? "An unexpected error occurred. Please try again later."
            : exception.Message;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(payload);
    }
}