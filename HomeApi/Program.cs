using System.Data.Common;
using HomeApi.Data;
using HomeApi.Models;
using HomeApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HomeApiContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("FlintHomeDb"))
        .EnableSensitiveDataLogging();
});

// Add services to the container.
builder.Services.AddHostedService<MqttService>();

builder.Services.AddTransient<ITempService, TempService>();

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World");
app.MapGet("/temp/current/{location}",
    async (string location, ITempService tempService) => await tempService.GetCurrentTemp(location));
app.MapGet("/temp/currentDay/{location}",
    async (string location, ITempService tempService) => await tempService.GetCurrentDayTemp(location));
//app.MapControllers();

app.Run();