using Application.Metadata;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Application.BusinessLogic.ProductLogic.Validators;

namespace Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddMediatr(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyInfo.Assembly));
            services.AddValidatorsFromAssemblyContaining<ProductValidator>();
            return services;
        }
    }
}
