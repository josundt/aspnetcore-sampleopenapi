using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.SampleOpenApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    private readonly IHostEnvironment _environment;
    private readonly Lazy<IExceptionHandlerFeature> _exceptionHandlerFeatureLazy;

    public ErrorController(IHostEnvironment environment)
    {
        _environment = environment;
        _exceptionHandlerFeatureLazy = new(() => this.HttpContext.Features.Get<IExceptionHandlerFeature>()!);
    }

    private IExceptionHandlerFeature ExceptionHandlerFeature => _exceptionHandlerFeatureLazy.Value;

    [AllowAnonymous]
    [Route("error")]
    public IActionResult HandleError()
    {
        var revealExceptionDetails = _environment.IsDevelopment();

        var ex = ExceptionHandlerFeature?.Error;
        if (ex == null)
        {
            return this.Problem();
        }

        // Track exception

        return this.Problem(
            title: revealExceptionDetails
                ? $"{ex.GetType().FullName}: {ex.Message}"
                : $"An unexpected error occured",
            detail: revealExceptionDetails
                ? ex.StackTrace
                : null,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
}
