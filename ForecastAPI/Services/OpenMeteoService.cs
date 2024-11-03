using FluentResults;
using ForecastAPI.DTOs;
using ForecastAPI.Errors;

namespace ForecastAPI.Services;

public interface IOpenMeteoService
{
    Task<Result<OpenMeteoResponse?>> GetForecast(double latitude, double longitude);
}

public class OpenMeteoService(HttpClient httpClient, ILogger<OpenMeteoService> logger)
    : IOpenMeteoService
{
    public async Task<Result<OpenMeteoResponse?>> GetForecast(double latitude, double longitude)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<OpenMeteoResponse>($"forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,wind_speed_10m");
            return Result.Ok(result);
        }
        catch (Exception)
        {
            return Result.Fail(OpenMeteoErrors.CouldNotFetch);
        }
    }
}
