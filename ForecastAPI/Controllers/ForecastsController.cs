using ForecastAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ForecastAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForecastsController(IForecastService forecastService, ILogger<ForecastsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetForecast(double latitude, double longitude)
    {
        if (latitude == 0 && longitude == 0)
        {
            return Ok(forecastService.GetAllForecasts());
        }

        if (latitude == 0 || longitude == 0)
        {
            return BadRequest("You have to specify both latitude and longitude");
        }

        var forecast = forecastService.GetForecast(latitude, longitude);

        if (forecast == null)
        {
            logger.LogInformation($"No forecast found for latitude = {latitude} and longitude = {longitude}");

            var addForecastResult = await forecastService.AddForecast(latitude, longitude);

            if (addForecastResult.IsSuccess)
            {
                logger.LogInformation($"Succesfully added forecast to DB for latitude = {latitude} and longitude = {longitude}");
                return Ok(addForecastResult.Value);
            }

            return BadRequest();
        }

        if (forecast.CreatedAt > forecast.CreatedAt.AddHours(12))
        {
            logger.LogInformation($"Found forecast but too old for latitude = {latitude} and longitude = {longitude}");

            var addForecastResult = await forecastService.AddForecast(latitude, longitude);

            if (addForecastResult.IsFailed)
            {
                return BadRequest();
            }

            var updateResult = forecastService.UpdateForecast(forecast.Id, addForecastResult.Value.Temperature, addForecastResult.Value.WindSpeed);

            if (updateResult.IsFailed)
            {
                return BadRequest();
            }
        }

        return Ok(forecast);
    }

    [HttpDelete]
    public IActionResult Delete([BindRequired]double latitude, [BindRequired]double longitude)
    {
        var deleteResult = forecastService.DeleteForecast(latitude, longitude);

        if (deleteResult.IsFailed)
        {
            return BadRequest("Could not delete forecast. No such forecast exists");
        }

        return Ok();
    }
}
