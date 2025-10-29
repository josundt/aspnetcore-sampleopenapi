using System.Text.Json.Serialization;

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
    public required DateOnly Date { get; init; }

    public required int TemperatureC { get; init; }

    public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

    public required string Summary { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))] // NB! This should not be necessary when API default serialization options has JsonStringEnumConverer
    public Status? PrevStatus { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))] // NB! This should not be necessary when API default serialization options has JsonStringEnumConverer
    public Status CurrStatus { get; init; }

    public required Person CreatedBy { get; init; }

    public Person? ModifiedBy { get; init; }
}
