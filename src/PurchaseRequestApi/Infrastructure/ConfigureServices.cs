using Infrastructure.CurrencyRatesService;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddDb(this IServiceCollection services, IHostEnvironment env)
        {
            using var serviceProvider = services.BuildServiceProvider();

            var dbOption = serviceProvider.GetRequiredService<IOptions<DbOptions>>().Value;
            string connectionString;

            if (env.IsProduction())
                connectionString = $"Host={dbOption.Host};Database={dbOption.Database};Port=5432;Username={dbOption.Username};Password={dbOption.Password};SSL Mode=Require;";
            else
                connectionString = $"Host={dbOption.Host};Database={dbOption.Database};Username={dbOption.Username};Password={dbOption.Password}";

            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(connectionString));

            

            return services;
        }

      
        public static IServiceCollection AddCurrencyRatesService(this IServiceCollection services)
        {

            services.AddHttpClient<CurrencyExchangeService>(client =>
            {
                client.BaseAddress = new Uri("https://api.exchangeratesapi.io/v1/");
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            services.AddSingleton<CurrencyExchangeService>();
            services.AddSingleton<ICurrencyExchangeService>(sp => sp.GetRequiredService<CurrencyExchangeService>());
            services.AddHostedService(sp => sp.GetRequiredService<CurrencyExchangeService>());

            return services;
        }
    }
}
