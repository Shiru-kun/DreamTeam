using DreamTeamAPI.Interfaces;
using DreamTeamAPI.Models;
using DreamTeamAPI.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamTeam.UnitTests
{
    internal class TestTeamService
    {
        private Mock<DreamTeamContext> _contextMock;
        private TeamService _teamService;
        private Mock<DbSet<Team>> _dbSetMock;
        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<DreamTeamContext>();
            _teamService = new TeamService(_contextMock.Object);
            _dbSetMock = new Mock<DbSet<Team>>();
            _contextMock.Setup(c => c.Teams).Returns(_dbSetMock.Object);
        }

        [Test]
        public async Task Should_Return_All_Team_Members()
        {
            var teams = new List<Team>
            {
                new Team { Id = Guid.NewGuid(), Fullname = "EM" },
                new Team { Id = Guid.NewGuid(), Fullname = "VX" },
                new Team { Id = Guid.NewGuid(), Fullname = "MK" },
                new Team { Id = Guid.NewGuid(), Fullname = "FK" }
            }.AsQueryable();
            _contextMock.Setup(c => c.Teams).ReturnsDbSet(teams);
            var result = await _teamService.GetAll();
            result.Should().BeEquivalentTo(teams);
            result.Count().Should().Be(4);
        }
        [Test]
        public async Task Should_Return_One_Member_By_Id()
        {
            Guid id = Guid.NewGuid();
            var teamMember = new Team { Id = id, Fullname = "EM" };
            _contextMock.Setup(c => c.Teams.FindAsync(id)).ReturnsAsync(teamMember);

            var result = await _teamService.GetById(id);
            result.Fullname.Should().Be("EM");
        }
        [Test]
        public async Task Should_Return_NoMember_When_Does_Not_Exist()
        {

            Team? teamMember = null;
            _contextMock.Setup(c => c.Teams.FindAsync(It.IsAny<Guid>)).ReturnsAsync(teamMember);

            var result = await _teamService.GetById(Guid.NewGuid());
            result.Should().BeOfType<NullTeam>();
        }

        [Test]
        public void Should_Return_Guid_When_Saved()
        {
            Team team = new Team() { Fullname = "Focas", JobTitle ="Fullstack Developer"};
            _dbSetMock.Setup(m => m.Add(team)).Verifiable();

            var result = _teamService.Save(team);
            result.Should().NotBeEmpty();
        }

        [Test]
        public void Should_Return_Guid_Empty_When_Exception_Occured()
        {
            Team team = new Team() { Fullname = "Focas", JobTitle = "Fullstack Developer" };
            _dbSetMock.Setup(m => m.Add(It.IsAny<Team>())).Throws(new Exception());
            var result = _teamService.Save(team);
            result.Should().Be(Guid.Empty);
        }

        [Test]
        public void Should_Return_True_When_Updated()
        {
            _dbSetMock.Setup(m => m.Update(It.IsAny<Team>())).Verifiable();
            var result = _teamService.Update(It.IsAny<Team>());
            result.Should().BeTrue();

        }
        [Test]
        public void Should_Return_False_When_Exception_Is_Thrown()
        {
            
            _dbSetMock.Setup(m => m.Update(It.IsAny<Team>())).Throws(new Exception());

            var result = _teamService.Update(It.IsAny<Team>());

            result.Should().BeFalse();
        }

        [Test]
        public async Task Should_Return_True_When_Team_Is_Deleted()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Fullname = "Team 1" };

            _dbSetMock.Setup(m => m.FindAsync(teamId)).ReturnsAsync(team);
            _dbSetMock.Setup(m => m.Remove(It.IsAny<Team>())).Verifiable();

            var result = await _teamService.Delete(teamId);

            result.Should().BeTrue();
            _dbSetMock.Verify(m => m.Remove(It.Is<Team>(t => t.Id == teamId)), Times.Once);
        }
        [Test]
        public async Task Should_Return_False_When_Delete_Exception_Is_Thrown()
        {
            var teamId = Guid.NewGuid();

            _dbSetMock.Setup(m => m.FindAsync(teamId)).ThrowsAsync(new Exception());

            var result = await _teamService.Delete(teamId);

            result.Should().BeFalse();
        }

    }
}
