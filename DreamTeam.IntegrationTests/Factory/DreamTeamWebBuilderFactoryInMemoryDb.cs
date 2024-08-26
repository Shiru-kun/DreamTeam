using DreamTeamAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamTeam.IntegrationTests.Factory
{
    internal class DreamTeamWebBuilderFactoryInMemoryDb : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services => {
                services.RemoveAll(typeof(DbContextOptions<DreamTeamContext>));
                services.AddDbContext<DreamTeamContext>(options => {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                });

                var context = CreateDbContext(services);
                context.Database.EnsureDeleted();
            });
            
        }
        public static DreamTeamContext CreateDbContext(IServiceCollection services)
        { return services.
                    BuildServiceProvider()
                    .GetRequiredService<DreamTeamContext>();
        }
    }
}
