using Application.Metadata;
using Infrastructure.CurrencyRatesService;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Reflection.Metadata;

namespace Testing
{
    [TestFixture]
    public abstract class HandlerTestBase
    {
        protected AppDbContext Database = default!;
        protected IMediator Mediator = default!;

        [SetUp]
        public async Task SetUp()
        {
            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AssemblyInfo.Assembly));
            services.AddLogging(b => b.AddConsole());
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(Tests.ConnectionString));

            services.AddSingleton<ICurrencyExchangeService>(new FakeCurrencyExchangeService());

            var provider = services.BuildServiceProvider();
            Mediator = provider.GetRequiredService<IMediator>();
            Database = provider.GetRequiredService<AppDbContext>();

            await SeedAsync(Database);
            await Database.SaveChangesAsync();
        }

        protected virtual Task SeedAsync(AppDbContext db) => Task.CompletedTask;

        [TearDown]
        public async Task TearDown()
        {
            await Tests.ResetDatabase();

            Database.Dispose();
        }
    }
}
