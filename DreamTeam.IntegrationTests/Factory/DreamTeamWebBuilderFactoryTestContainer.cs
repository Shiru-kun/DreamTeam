using DreamTeamAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Testcontainers.MsSql;

namespace DreamTeam.IntegrationTests.Factory
{
    public class DreamTeamWebBuilderFactoryTestContainer : WebApplicationFactory<Program>
    {
        private readonly MsSqlContainer _dbContainer;
        public DreamTeamWebBuilderFactoryTestContainer() {
            _dbContainer = new MsSqlBuilder()
            .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<DreamTeamContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DreamTeamContext>(options =>
                    options.UseSqlServer(_dbContainer.GetConnectionString()));
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
        }
    }
}