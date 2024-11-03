using ForecastAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForecastAPI.Database;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Forecast> Forecasts { get; set; }
}
