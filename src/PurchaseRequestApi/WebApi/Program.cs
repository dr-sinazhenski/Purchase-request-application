using Application;
using Infrastructure;
using WebApi;
using AutoWrapper;
using Serilog;

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
