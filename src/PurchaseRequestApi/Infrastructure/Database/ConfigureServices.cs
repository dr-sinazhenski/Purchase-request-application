using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Database
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddDb(this IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();

            var dbOption = serviceProvider.GetRequiredService<IOptions<DbOptions>>().Value;
            var connectionString = $"Host={dbOption.Host};Database={dbOption.Database};Username={dbOption.Username};Password={dbOption.Password}";

            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(connectionString));

            return services;
        }
    }
}
