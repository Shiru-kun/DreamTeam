using DreamTeamAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;


namespace DreamTeam.IntegrationTestXUnit
{
    public class DreamTeamApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer;
        public DreamTeamApplicationFactory() {
            _dbContainer = new MsSqlBuilder()
         // .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
          //.WithPassword("Strong_password_123!")
          .Build();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services => {
                services.RemoveAll(typeof(DbContextOptions<DreamTeamContext>));
                services.AddDbContext<DreamTeamContext>(ops=> {
                    string connectionString = _dbContainer.GetConnectionString();
                    ops.UseSqlServer(connectionString);
                });
            });
        }
        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
             using(var scope = Services.CreateScope())
            {
                var scoped = scope.ServiceProvider; 
                var context = scoped.GetRequiredService<DreamTeamContext>();
                await context.Database.EnsureCreatedAsync();   
                await context.SaveChangesAsync();
            }
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
        }
    }
}
