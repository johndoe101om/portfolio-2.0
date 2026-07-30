namespace Portfolio.Api.Middleware;

/// <summary>
/// Logs HTTP requests with method, path, status, and duration.
/// Sanitises query strings to avoid logging sensitive values.
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private static readonly string[] SensitiveHeaders = ["Authorization", "Cookie", "X-Api-Key"];

    public async Task InvokeAsync(HttpContext ctx)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            await next(ctx);
        }
        finally
        {
            sw.Stop();
            var path = ctx.Request.Path.Value ?? "/";
            // Skip health check noise
            if (!path.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms",
                    ctx.Request.Method,
                    path,
                    ctx.Response.StatusCode,
                    sw.ElapsedMilliseconds);
            }
        }
    }
}
