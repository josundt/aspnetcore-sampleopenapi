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
    /// <returns>The collection of <see cref="WeatherForecast"/>s</returns>
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [HttpGet(Name = nameof(GetWeatherForcasts))]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public IEnumerable<WeatherForecast> GetWeatherForcasts()
    {
        return [.. Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)],
            CreatedBy = new() { Name = "Foo" }
        })];
    }

    /// <summary>
    /// Get weather forcast by date
    /// </summary>
    /// <param name="date">The the weather forecast date</param>
    /// <returns>The <see cref="WeatherForecast"/></returns>
    [ApiVersion("2.0")]
    [HttpGet("{date}", Name = nameof(GetWeatherForcast))]
    [ProducesResponseType(typeof(WeatherForecast), StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public WeatherForecast GetWeatherForcast(DateOnly date)
    {
        return new WeatherForecast
        {
            Date = date,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)],
            CreatedBy = new() { Name = "Foo" }
        };
    }
}
