using DreamTeam.IntegrationTests.Factory;
using DreamTeamAPI.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using NUnit.Framework;
using Microsoft.AspNetCore.TestHost;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace DreamTeam.IntegrationTests
{
    public class TeamsControllerIntegrationTests
    {
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new DreamTeamWebBuilderFactoryInMemoryDb();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task Should_Get_Empty_List_Of_Team_Members()
        {
            var response = await _client.GetAsync("/api/teams");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<List<Team>>();
            result.Count.Should().Be(0);
        }

        [Test]
        public async Task Should_Return_Ok_When_Create_Team_Member()
        {
            var teamMember = new Team { Fullname = "EM", JobTitle = "Developer" };
            var response = await _client.PostAsJsonAsync("/api/teams", teamMember);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Test]
        public async Task Should_Return_BadRequest_When_Create_Team_Member_Not_Completed_Data()
        {
            var teamMember = new Team { Fullname = "EM" };
            var response = await _client.PostAsJsonAsync("/api/teams", teamMember);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
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
                                        options.UseInMemoryDatabase("Should_Get_Team_By_Id");
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
               await  context.SaveChangesAsync();
            }
            var client = factory.CreateClient();
            var response = await client.GetAsync($"/api/teams/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var team = await response.Content.ReadFromJsonAsync<Team>();
            team?.Fullname.Should().Be("EM");
        }

        [Test]
        public async Task Should_Return_NotFound_For_NoN_Existent_Team()
        {
            var nonExistentTeamId = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/teams/{nonExistentTeamId}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_Update_Team()
        {
            Guid id = new Guid("babc062c-15aa-483c-a1f3-357deb08b0f7");
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<DreamTeamContext>));

                    services.AddDbContext<DreamTeamContext>(options =>
                    {
                        options.UseInMemoryDatabase("Should_Update_Team");
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
            var team = new Team() { Fullname = "Fk", Id = id, JobTitle = "Fullstack developer" };
            var client = factory.CreateClient();
            var response = await client.PutAsJsonAsync("/api/teams", team);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<bool>();
            result.Should().BeTrue();
        }

        [Test]
        public async Task Should_Delete_Team()
        {
            Guid id = new Guid("babc062c-15aa-483c-a1f3-357deb08b0f7");
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<DreamTeamContext>));

                    services.AddDbContext<DreamTeamContext>(options =>
                    {
                        options.UseInMemoryDatabase("Should_Delete_Team");
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
            var response = await client.DeleteAsync("/api/teams/"+id);
            var result = await response.Content.ReadFromJsonAsync<bool>();
            result.Should().BeTrue();


        }
    }
}
