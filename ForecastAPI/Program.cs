using ForecastAPI.Database;
using ForecastAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IOpenMeteoService, OpenMeteoService>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast");
});

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlite("Data Source=forecasts.db");
});

builder.Services.AddControllers();
// builder.Services.AddTransient<IOpenMeteoService, OpenMeteoService>();
builder.Services.AddTransient<IForecastService, ForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
