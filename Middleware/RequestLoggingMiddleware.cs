namespace copilot_api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var originalBodyStream = context.Response.Body;

        try
        {
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // Log the incoming request
            _logger.LogInformation(
                "Request: {Method} {Path} - Started at {StartTime}",
                context.Request.Method,
                context.Request.Path,
                startTime);

            await _next(context);

            // Copy the response back to the original stream
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            // Log the response
            _logger.LogInformation(
                "Response: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - Completed at {EndTime}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds,
                endTime);
        }
        catch (Exception ex)
        {
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            _logger.LogError(
                ex,
                "Error: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - Error: {ErrorMessage}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds,
                ex.Message);

            // Restore the original body stream
            context.Response.Body = originalBodyStream;
            throw;
        }
    }
} 