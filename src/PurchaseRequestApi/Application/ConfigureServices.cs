using Application.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddMediatr(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyInfo.Assembly));

            return services;
        }
    }
}
