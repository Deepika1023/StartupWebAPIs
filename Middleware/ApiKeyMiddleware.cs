using StartupWebAPIs.Data;
using Microsoft.EntityFrameworkCore;
namespace StartupWebAPIs.Middleware
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeaderName = "X-API-KEY";

        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            //// Skip API key validation for Auth endpoints
            //if (context.Request.Path.StartsWithSegments("/api/auth"))
            //{
            //    await _next(context);
            //    return;
            //}
            var path = context.Request.Path.Value?.ToLower();

            if (path!.StartsWith("/api/auth") ||
                path.StartsWith("/swagger") ||
                path.StartsWith("/api/apikey"))
            {
                await _next(context);
                return;
            }

            // Skip Swagger
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Check if header exists
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await context.Response.WriteAsync("API Key is missing.");

                return;
            }
            // Convert header value to string
            string apiKeyValue = extractedApiKey.ToString();


            // Validate API key
            var apiKey = await dbContext.ApiKeys
                .FirstOrDefaultAsync(x =>
                    x.Key == apiKeyValue &&
                    x.IsActive);

            if (apiKey == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await context.Response.WriteAsync("Invalid API Key.");

                return;
            }

            await _next(context);
        }
    }
}
