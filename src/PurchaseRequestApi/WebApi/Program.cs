using Application;
using Infrastructure;
using Serilog;
using WebApi;
using WebApi.Middleware;
using Microsoft.OpenApi;


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

builder.Services.AddSingleton(log);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { 
    Title = "My API", 
    Version = "v1" 
  });
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
    In = ParameterLocation.Header, 
    Description = "Please insert JWT with Bearer into field",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey 
  });
  c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

builder.ConfigureProjectsOptions();
builder.Services.AddDb();
builder.Services.AddMediatr();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

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