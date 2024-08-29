using DotNet.Testcontainers.Containers;
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
using NUnit.Framework;
using Docker.DotNet.Models;

namespace DreamTeam.IntegrationTests
{
    [TestFixture]
    public class TestCrudDreamTeamTestContainers
    {
        private MsSqlContainer _dbContainer;
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private DreamTeamContext _context;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _dbContainer = new MsSqlBuilder().Build();
            await _dbContainer.StartAsync();
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
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
            var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DreamTeamContext>();
            await context.Database.EnsureCreatedAsync();
            await context.SaveChangesAsync();
            _context = context;
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _context.Database.EnsureDeletedAsync();
            _client.Dispose();
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
            _factory.Dispose();
        }

        [Test]
        public async Task Should_Get_Team_By_Id()
        {
            Guid id = Guid.NewGuid();
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DreamTeamContext>();
                context.Database.EnsureCreated();
                context.Teams.Add(
                    new Team() { Fullname = "EM", Id = id, JobTitle = "Developer" }
                );
                await context.SaveChangesAsync();
            }
            var response = await _client.GetAsync($"/api/teams/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var team = await response.Content.ReadFromJsonAsync<Team>();
            team?.Fullname.Should().Be("EM");
        }

        [Test]
        public async Task Should_Get_Empty_List_Of_Team_Members()
        {
            var response = await _client.GetAsync("/api/teams");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<List<Team>>();
            result.Count.Should().BePositive();
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
        public async Task Should_Return_NotFound_For_NoN_Existent_Team()
        {
            var nonExistentTeamId = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/teams/{nonExistentTeamId}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_Update_Team()
        {
            Guid id = Guid.NewGuid();
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DreamTeamContext>();
                context.Database.EnsureCreated();
                context.Teams.Add(
                    new Team() { Fullname = "EM", Id = id, JobTitle = "Developer" }
                );
                await context.SaveChangesAsync();
            }
            var team = new Team() { Fullname = "Fk", Id = id, JobTitle = "Fullstack developer" };
            var response = await _client.PutAsJsonAsync("/api/teams", team);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<bool>();
            result.Should().BeTrue();
        }
        [Test]
        public async Task Should_Delete_Team()
        {
            Guid id = Guid.NewGuid();
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DreamTeamContext>();
                context.Database.EnsureCreated();
                context.Teams.Add(
                    new Team() { Fullname = "EM", Id = id, JobTitle = "Developer" }
                );
                await context.SaveChangesAsync();
            }
            var response = await _client.DeleteAsync("/api/teams/" + id);
            var result = await response.Content.ReadFromJsonAsync<bool>();
            result.Should().BeTrue();
        }
    }
}
