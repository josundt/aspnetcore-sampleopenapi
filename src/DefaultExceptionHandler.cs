using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.SampleOpenApi;

public class DefaultExceptionHandler(IHostEnvironment environment, ILogger<DefaultExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        this.LogException(exception.Message, exception);

        var revealExceptionDetails = environment.IsDevelopment();

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = revealExceptionDetails
                ? $"{exception.GetType().FullName}: {exception.Message}"
                : $"An unexpected error occured",
            Detail = revealExceptionDetails
                ? exception.StackTrace
                : null
        };

        // CustomizeProblemDetails will automatically add traceId, timestamp, etc.
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Exception handled
    }

    private void LogException(string message, Exception exception)
    {
        _logExceptionDelegate(logger, message, exception);
    }

    private static readonly Action<ILogger, string, Exception?> _logExceptionDelegate = LoggerMessage.Define<string>(
        LogLevel.Error,
        new EventId(),
        "Unhandled exception occurred: {Message}"
    );
}
