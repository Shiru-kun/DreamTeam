using DreamTeam.IntegrationTests.Factory;
using DreamTeamAPI.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.MsSql;

namespace DreamTeam.IntegrationTests
{
    public class IntegrationTestCrudDreamTeamTestContainers
    {
        MsSqlContainer _dbContainer = new MsSqlBuilder()
          .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
          .WithPassword("Strong_password_123!")
          .Build();
        [SetUp] public async Task SetUp() { await _dbContainer.StartAsync();}
        [TearDown] public async Task TearDown() { await _dbContainer.StopAsync(); } 
        public async Task Should_Get_Team_By_Id()
        {
            Guid id = new Guid("babc062c-15aa-483c-a1f3-357deb08b0f7");
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<DreamTeamContext>));

                    services.AddDbContext<DreamTeamContext>(options =>
                    {
                        options.UseSqlServer(_dbContainer.GetConnectionString());
                    });
                });
            });
            using (var scope = factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DreamTeamContext>();
                context.Database.EnsureCreated();
                context.Teams.Add(
                    new Team() { Fullname = "EM", Id = id, JobTitle = "Developer" }
                );
                await context.SaveChangesAsync();
            }
            var client = factory.CreateClient();
            var response = await client.GetAsync($"/api/teams/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var team = await response.Content.ReadFromJsonAsync<Team>();
            team?.Fullname.Should().Be("EM");
        }

    }
}
