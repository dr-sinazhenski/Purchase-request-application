using Application;
using Infrastructure;
using Serilog;
using WebApi;

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
    
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.ConfigureProjectsOptions();
builder.Services.AddDb();
builder.Services.AddMediatr();

builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, services, cfg) => cfg
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
