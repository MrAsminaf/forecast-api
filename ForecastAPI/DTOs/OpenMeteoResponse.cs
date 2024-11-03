using System.Text.Json.Serialization;

namespace ForecastAPI.DTOs;

public class OpenMeteoResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Current Current { get; set; } = new();
}

public class Current
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed { get; set; }
}
