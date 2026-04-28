using Application;
using Infrastructure;
using WebApi;
using AutoWrapper;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

using var log = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();
    
builder.Host.UseSerilog(log);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.ConfigureProjectsOptions();
builder.Services.AddDb();
builder.Services.AddMediatr();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApiResponseAndExceptionWrapper();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
