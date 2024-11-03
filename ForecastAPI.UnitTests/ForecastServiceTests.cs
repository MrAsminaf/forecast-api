using ForecastAPI.DTOs;
using NSubstitute;
using ForecastAPI.Services;
using ForecastAPI.Errors;
using Xunit;
using FluentAssertions;
using FluentResults;
using ForecastAPI.Entities;

namespace ForecastAPI.UnitTests;

public class ForecastServiceTests : UnitTestsBase
{
    private readonly IOpenMeteoService _openMeteoServiceMock;

    public ForecastServiceTests()
    {
        _openMeteoServiceMock = Substitute.For<IOpenMeteoService>();
    }

    [Fact]
    public async Task AddForecast_ReturnsSuccess_WhenForecastIsAdded()
    {
        // Arrange
        var latitude = 10.0;
        var longitude = 20.0;
        _openMeteoServiceMock.GetForecast(latitude, longitude)
            .Returns(new OpenMeteoResponse
            {
                Current = new Current
                {
                    Temperature = 14
                }
            });
        using var ctx = CreateContext();
        var forecastService = new ForecastService(ctx, _openMeteoServiceMock);

        // Act
        var result = await forecastService.AddForecast(latitude, longitude);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AddForecast_ReturnsFailure_WhenOpenMeteoServiceFails()
    {
        // Arrange
        var latitude = 10.0;
        var longitude = 20.0;

        _openMeteoServiceMock.GetForecast(latitude, longitude)
            .Returns(Result.Fail(OpenMeteoErrors.CouldNotFetch));
        using var ctx = CreateContext();
        var forecastService = new ForecastService(ctx, _openMeteoServiceMock);

        // Act
        var result = await forecastService.AddForecast(latitude, longitude);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(result.Errors.First().Message, ForecastErrors.CouldNotFetch.Message);
    }
    
    [Fact]
    public void GetAllForecasts_ReturnsAllForecasts()
    {
        // Arrange
        var forecasts = new List<Forecast>
            {
                new Forecast { Latitude = 10.0, Longitude = 20.0, Temperature = "15.0", WindSpeed = "10.0" },
                new Forecast { Latitude = 30.0, Longitude = 40.0, Temperature = "20.0", WindSpeed = "5.0" }
            };
    
        using var ctx = CreateContext();
        ctx.AddRange(forecasts);
        ctx.SaveChanges();

        var forecastService = new ForecastService(ctx, _openMeteoServiceMock);
    
        // Act
        var result = forecastService.GetAllForecasts();
    
        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, f => f.Latitude == 10.0 && f.Longitude == 20.0);
    }
    
    [Fact]
    public void GetForecast_ReturnsForecast_WhenForecastExists()
    {
        // Arrange
        var latitude = 10.0;
        var longitude = 20.0;
        var forecast = new Forecast 
        { 
            Latitude = latitude,
            Longitude = longitude
        };
    
        using var ctx = CreateContext();
        ctx.Add(forecast);
        ctx.SaveChanges();

        var forecastService = new ForecastService(ctx, _openMeteoServiceMock);
    
        // Act
        var result = forecastService.GetForecast(latitude, longitude);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(forecast, result);
    }
    
    // [Fact]
    // public void UpdateForecast_ReturnsSuccess_WhenForecastExists()
    // {
    //     // Arrange
    //     var forecastId = 1;
    //     var forecast = new Forecast { Id = forecastId, Temperature = "10.0", WindSpeed = "5.0" };
    //     var forecasts = new List<Forecast> { forecast }.AsQueryable().BuildMockDbSet();
    //     _contextMock.Setup(ctx => ctx.Forecasts).Returns(forecasts.Object);
    //
    //     // Act
    //     var result = _forecastService.UpdateForecast(forecastId, "15.0", "10.0");
    //
    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal("15.0", forecast.Temperature);
    //     Assert.Equal("10.0", forecast.WindSpeed);
    //     _contextMock.Verify(ctx => ctx.SaveChanges(), Times.Once);
    // }
    //
    // [Fact]
    // public void UpdateForecast_ReturnsFailure_WhenForecastDoesNotExist()
    // {
    //     // Arrange
    //     var forecastId = 99;
    //     var forecasts = new List<Forecast>().AsQueryable().BuildMockDbSet();
    //     _contextMock.Setup(ctx => ctx.Forecasts).Returns(forecasts.Object);
    //
    //     // Act
    //     var result = _forecastService.UpdateForecast(forecastId, "15.0", "10.0");
    //
    //     // Assert
    //     Assert.True(result.IsFailed);
    //     Assert.Equal("Could not find an existing forecast", result.Errors.First().Message);
    // }
}

