using System.Net;
using System.Text.Json;

namespace StartupWebAPIs.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found.");

                await WriteResponse(context,
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access.");

                await WriteResponse(context,
                    HttpStatusCode.Unauthorized,
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled Exception. Path: {Path}",
                    context.Request.Path);

                await WriteResponse(context,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.",
                    _environment.IsDevelopment() ? ex.Message : null);
            }
        }

        private async Task WriteResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string message,
        string? details = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                Success = false,
                Message = message,
                StatusCode = (int)statusCode,
                Details = details
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}