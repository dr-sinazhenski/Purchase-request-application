using Application;
using Infrastructure;
using Serilog;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

using var log = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File(
    path: "logs/log-.txt",
    rollingInterval: RollingInterval.Day, 
    retainedFileCountLimit: 7            
    )
    .CreateLogger();
    
builder.Host.UseSerilog(log);

builder.Services.AddControllers();
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
