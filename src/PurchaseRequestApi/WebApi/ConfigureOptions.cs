using Application.Options;
using Infrastructure.CurrencyRatesService;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApi
{
    public static class ConfigureOptions
    {
        public static void ConfigureProjectsOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<DbOptions>(
                builder.Configuration.GetSection(nameof(DbOptions)));

            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(nameof(JwtOptions)));

            builder.Services.Configure<CurrencyExchangeOptions>(
                builder.Configuration.GetSection(nameof(CurrencyExchangeOptions)));
        }
    }
}
