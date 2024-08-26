using DreamTeamAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Json;

namespace DreamTeam.IntegrationTestsV2
{
    public class Tests
    {

        [Test]
        public async Task Test1()
        {
            //arrange
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    services.RemoveAll(typeof(DbContextOptions<DreamTeamContext>));
                    services.AddDbContext<DreamTeamContext>(options => {
                        options.UseInMemoryDatabase("test");
                           // options.UseSqlServer();
                        });
                });
            });
            using(var scope = factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DreamTeamContext>();    
                context.Database.EnsureCreated();
                context.Teams.Add( new Team() { Fullname="EM", Id = Guid.NewGuid(),JobTitle="Developer"  });  
                context.Teams.Add( new Team() { Fullname="Fk", Id = Guid.NewGuid(),JobTitle="Developer"  });  
                context.SaveChanges();
            }
            var client = factory.CreateClient();

            //Act 
            var response = await client.GetAsync($"/api/teams");
            var result = await response.Content.ReadFromJsonAsync<List<Team>>();
           
            // Assert
            Assert.NotNull( result );   
            
        }
    }
}