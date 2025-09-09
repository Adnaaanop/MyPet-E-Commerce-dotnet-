using System.Net;
using System.Text.Json;

namespace MyApp.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception");

                var statusCode = HttpStatusCode.InternalServerError;

                if (ex is KeyNotFoundException)
                    statusCode = HttpStatusCode.NotFound;
                else if (ex is UnauthorizedAccessException)
                    statusCode = HttpStatusCode.Unauthorized;
                else if (ex is ArgumentException || ex is InvalidOperationException)
                    statusCode = HttpStatusCode.BadRequest;

                var response = new
                {
                    success = false,
                    message = ex.Message,
                    details = appIsDevelopment(context) ? ex.StackTrace : null,
                    statusCode = (int)statusCode,
                    traceId = context.TraceIdentifier
                };

                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }

        private bool appIsDevelopment(HttpContext context)
        {
            var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
            return env.IsDevelopment();
        }
    }

}
