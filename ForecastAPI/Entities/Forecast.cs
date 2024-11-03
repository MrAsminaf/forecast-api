namespace ForecastAPI.Entities;

public class Forecast
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Temperature { get; set; } = string.Empty;
    public string WindSpeed { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
