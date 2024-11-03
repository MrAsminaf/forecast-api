using FluentResults;

namespace ForecastAPI.Errors;

public static class OpenMeteoErrors
{
    public static readonly Error CouldNotFetch = new("Could not fetch data from OpenMeteo service");
}