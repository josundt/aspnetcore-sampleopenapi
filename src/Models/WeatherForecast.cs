using System.ComponentModel.DataAnnotations;

namespace AspNetCore.SampleOpenApi.Models;


public enum Status
{
    Active,
    Inactive
}

public class Person
{
    public required string Name { get; init; }
}

public class WeatherForecast
{
    public required Guid Id { get; init; }

    public required DateOnly Date { get; init; }

    [Range(-274, 5000)]
    public required int TemperatureC { get; init; }

    public long TemperatureF => 32 + (long)(this.TemperatureC / 0.5556);

    [EmailAddress]
    [Required]
    public string? ReportedBy { get; init; }

    [Length(1, 255)]
    public required string Summary { get; init; }

    //[JsonConverter(typeof(JsonStringEnumConverter))] // NB! This should not be necessary when API default serialization options has JsonStringEnumConverer
    public Status? PrevStatus { get; init; }

    //[JsonConverter(typeof(JsonStringEnumConverter))] // NB! This should not be necessary when API default serialization options has JsonStringEnumConverer
    public required Status CurrStatus { get; init; }

    public required Person CreatedBy { get; init; }

    public Person? ModifiedBy { get; init; }

    public required Uri Url { get; init;  }
}
