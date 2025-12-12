using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.SampleOpenApi.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/errordemo")]
public class ErrorDemoController : ApiControllerBase
{
    [HttpGet("default", Name = nameof(GetDefaultError))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public bool GetDefaultError()
    {
        throw new InvalidOperationException("This is an unexpected error that should reflect the default openapi response");
    }

    [HttpGet("problem", Name = nameof(GetProblemDetails))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesDefaultResponseType]
    public ActionResult<bool> GetProblemDetails()
    {
        return this.Problem(statusCode: StatusCodes.Status409Conflict);
    }

    [HttpGet("validationproblem", Name = nameof(GetValidationProblemDetails))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesDefaultResponseType]
    public ActionResult<bool> GetValidationProblemDetails()
    {
        this.ModelState.AddModelError("errorKey", "This is a demo validation error");
        return this.ValidationProblem(this.ModelState);
    }
}
