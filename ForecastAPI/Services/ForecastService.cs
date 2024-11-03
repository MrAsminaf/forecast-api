using FluentResults;
using ForecastAPI.Database;
using ForecastAPI.Entities;
using ForecastAPI.Errors;
using Microsoft.EntityFrameworkCore;

namespace ForecastAPI.Services;

public interface IForecastService
{
    IEnumerable<Forecast> GetAllForecasts();
    Forecast? GetForecast(double latitude, double longitude);
    Task<Result<Forecast>> AddForecast(double latitude, double longitude);
    Result<Forecast> UpdateForecast(int id, string temperature, string windSpeed);
    Result DeleteForecast(double latitude, double longitude);
}

public class ForecastService(ApplicationContext ctx,
        IOpenMeteoService openMeteoService) : IForecastService
{
    public async Task<Result<Forecast>> AddForecast(double latitude, double longitude)
    {
        var openMeteoResult = await openMeteoService.GetForecast(latitude, longitude);

        if (openMeteoResult.IsFailed || openMeteoResult.Value == null)
        {
            return Result.Fail(ForecastErrors.CouldNotFetch);
        }

        var forecast = new Forecast
        {
            Latitude = latitude,
            Longitude = longitude,
            Temperature = openMeteoResult.Value.Current.Temperature.ToString(),
            WindSpeed = openMeteoResult.Value.Current.WindSpeed.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        ctx.Forecasts.Add(forecast);
        ctx.SaveChanges();

        return Result.Ok(forecast);
    }

    public IEnumerable<Forecast> GetAllForecasts()
    {
        return ctx.Forecasts.ToList();
    }

    public Forecast? GetForecast(double latitude, double longitude)
    {
        return ctx.Forecasts.Where(f => f.Latitude == latitude && f.Longitude == longitude)
            .SingleOrDefault();
    }

    public Result<Forecast> UpdateForecast(int id, string temperature, string windSpeed)
    {
        var forecast = ctx.Forecasts.Where(f => f.Id == id)
            .SingleOrDefault();

        if (forecast == null)
        {
            Result.Fail(ForecastErrors.CouldNotFind);
        }

        forecast.Temperature = temperature;
        forecast.WindSpeed = windSpeed;

        ctx.SaveChanges();
        return Result.Ok();
    }

    public Result DeleteForecast(double latitude, double longitude)
    {
        var forecast = GetForecast(latitude, longitude);

        if (forecast == null)
        {
            return Result.Fail("No such forecast exists");
        }

        ctx.Entry(forecast).State = EntityState.Deleted;
        ctx.SaveChanges();

        return Result.Ok();
    }
}
