using Asp.Versioning;
using AspNetCore.SampleOpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SampleOpenApi.Controllers;

namespace AspNetCore.SampleOpenApi.Controllers.v1_0;

[Route("api/errordemo")]
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
        ModelState.AddModelError("errorKey", "This is a demo validation error");
        return this.ValidationProblem(ModelState);
    }
}
