using Microsoft.AspNetCore.Diagnostics;
using Serilog;

public class GlobalExceptionHandler(RequestDelegate next) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Log.Error(exception, "Exception occurred during execution of {Path}: {Message}", httpContext.Request.Path,
            exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsync(
            "Unexpected error occurred. Try again later or look into logs for more information.", 
            cancellationToken);

        return true;
    }
}