using DreamTeamAPI.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace DreamTeam.IntegrationTestXUnit
{
    public class TestCrudDreamTeam(DreamTeamApplicationFactory factory) :IClassFixture<DreamTeamApplicationFactory>
    {
        private DreamTeamApplicationFactory _factory= factory;
        private HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Should_Get_Team_By_Id()
        {
            Guid id =Guid.NewGuid();
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

        [Fact]
        public async Task Should_Get_Empty_List_Of_Team_Members()
        {
            var response = await _client.GetAsync("/api/teams");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<List<Team>>();
            result.Count.Should().BePositive();
        }

        [Fact]
        public async Task Should_Return_Ok_When_Create_Team_Member()
        {
            var teamMember = new Team { Fullname = "EM", JobTitle = "Developer" };
            var response = await _client.PostAsJsonAsync("/api/teams", teamMember);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_Return_BadRequest_When_Create_Team_Member_Not_Completed_Data()
        {
            var teamMember = new Team { Fullname = "EM" };
            var response = await _client.PostAsJsonAsync("/api/teams", teamMember);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task Should_Return_NotFound_For_NoN_Existent_Team()
        {
            var nonExistentTeamId = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/teams/{nonExistentTeamId}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
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
            var client = factory.CreateClient();
            var response = await client.PutAsJsonAsync("/api/teams", team);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<bool>();
            result.Should().BeTrue();
        }
        [Fact]
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
            var client = factory.CreateClient();
            var response = await client.DeleteAsync("/api/teams/" + id);
            var result = await response.Content.ReadFromJsonAsync<bool>();
            result.Should().BeTrue();
        }
    }
    }
