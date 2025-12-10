using Asp.Versioning;
using AspNetCore.SampleOpenApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.SampleOpenApi.Controllers;

[Route("api/v{version:apiVersion}/forecasts")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "<Pending>")]
public class WeatherForecastController : ApiControllerBase
{
    private static readonly string[] _summaries = [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    /// <summary>
    /// Get weather forcasts
    /// </summary>
    /// <remarks>
    /// Gets the weather forecast for the next 5 days.
    /// </remarks>
    /// <returns>The collection of <see cref="WeatherForecast"/>s</returns>
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [HttpGet(Name = nameof(GetWeatherForcasts))]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "The weather forecast for the next 5 days.")]
    [ProducesDefaultResponseType]
    public IEnumerable<WeatherForecast> GetWeatherForcasts()
    {
        var id = Guid.NewGuid();
        return [.. Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Id = id,
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)],
            CreatedBy = new() { Name = "Foo" },
            CurrStatus = Status.Active,
            Url = new Uri($"https://foo.bar/{id}")
        })];
    }

    /// <summary>
    /// Get weather forcast by date
    /// </summary>
    /// <remarks>
    /// Gets the weather forecast for the specified date.
    /// </remarks>
    /// <param name="date">The the weather forecast date</param>
    /// <returns>The <see cref="WeatherForecast"/></returns>
    [ApiVersion("2.0")]
    [HttpGet("{date}", Name = nameof(GetWeatherForcast))]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "The weather forecast for the specified date.")]
    [ProducesDefaultResponseType]
    public WeatherForecast GetWeatherForcast(DateOnly date)
    {
        var id = Guid.NewGuid();
        return new WeatherForecast
        {
            Id = Guid.NewGuid(),
            Date = date,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)],
            CreatedBy = new() { Name = "Foo" },
            CurrStatus = Status.Active,
            Url = new Uri($"https://foo.bar/{id}")
        };
    }
}
