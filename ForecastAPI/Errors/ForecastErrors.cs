using FluentResults;

namespace ForecastAPI.Errors;

public static class ForecastErrors
{
    public static readonly Error CouldNotFetch = new("Could not fetch data from OpenMeteo service");
    public static readonly Error CouldNotFind = new("Could not find an existing forecast");
}
