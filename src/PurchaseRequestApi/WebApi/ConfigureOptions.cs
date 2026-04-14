using Infrastructure.Database;
using Microsoft.AspNetCore.SignalR;

namespace WebApi
{
    public static class ConfigureOptions
    {
        public static void ConfigureProjectsOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<DbOptions>(
                builder.Configuration.GetSection(nameof(DbOptions)));
        }
    }
}
