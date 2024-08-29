using Microsoft.Playwright.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamTeam.E2ETests
{

    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class TestCrudTeamsMvc : PageTest
    {
        private readonly string _baseUrl = "http://localhost:5283/TeamsMvc";
        private readonly string url = "http://localhost:5283/";


        private async Task CreateUser(string fullname, string jobTitle) 
        {
            await Page.GotoAsync($"{_baseUrl}/Create");
            await Page.FillAsync("input[name='Fullname']", fullname);
            await Page.FillAsync("input[name='JobTitle']", jobTitle);
            await Page.ClickAsync("input[type='submit']");
            await Expect(Page).ToHaveURLAsync(url);
        }

        [Test]
        public async Task Should_Not_Add_New_Team_Member_When_InvalidData()
        {
            await Page.GotoAsync($"{_baseUrl}/Create");
            var fullname = Faker.Name.FullName();

            await Page.FillAsync("input[name='Fullname']", fullname);

            await Page.ClickAsync("input[type='submit']");

            await Expect(Page).ToHaveURLAsync(url);
            await Expect(Page.Locator($@"table tbody tr:has-text('{fullname}')")).Not.ToBeVisibleAsync();
        }

        [Test]
        public async Task Should_Add_New_Team_Member()
        {
            await Page.GotoAsync($"{_baseUrl}/Create");
            var fullname = Faker.Name.FullName();

            await Page.FillAsync("input[name='Fullname']", fullname);
            await Page.FillAsync("input[name='JobTitle']", "Developer");

            await Page.ClickAsync("input[type='submit']");

            await Expect(Page).ToHaveURLAsync(url);
            await Expect(Page.Locator($@"table tbody tr:has-text('{fullname}')")).ToBeVisibleAsync();
        }

        [Test]
        public async Task Should_Display_Team_Members()
        {
            await Page.GotoAsync(_baseUrl);

            await Expect(Page.Locator("h1")).ToHaveTextAsync("Team members");

            var rows = await Page.Locator("table tbody tr").CountAsync();
            Assert.Greater(rows, 0, "Expected at least one team member in the list.");
        }
        [Test]
        public async Task Should_Display_Team_Member_Info()
        {
            await Page.GotoAsync(_baseUrl);
            await Page.Locator("a[href*='Details']").First.ClickAsync();
            await Expect(Page.Locator("h1")).ToHaveTextAsync("Details");
        }
        [Test]
        public async Task Should_Update_Team_Member()
        {
            var fullname = Faker.Name.FullName();
            var newFullname = $"{fullname} Updated";
            var jobTitle = "Developer";
            await CreateUser(fullname, jobTitle);

            await Page.GotoAsync(_baseUrl);
            var rowLocator = Page.Locator($"table tbody tr:has-text('{fullname}')");
            await Expect(rowLocator).ToHaveCountAsync(1);

            await rowLocator.Locator("a[href*='Edit']").ClickAsync();

            await Page.FillAsync("input[name='Fullname']", newFullname);

            await Page.ClickAsync("input[type='submit']");

            await Expect(Page).ToHaveURLAsync(url);
            await Expect(Page.Locator($"table tbody tr:has-text('{newFullname}')")).ToBeVisibleAsync();
        }

        [Test]
        public async Task Should_Remove_Team_Member()
        {
            var fullname = "DeleteUser";
            var jobTitle = "Developer";
            await CreateUser(fullname, jobTitle);

            await Page.GotoAsync(_baseUrl);
            var rowLocator = Page.Locator($"table tbody tr:has-text('{fullname}')");

            await rowLocator.Locator("a[href*='Delete']").First.ClickAsync();

            await Page.ClickAsync("input[type='submit']");

            await Expect(rowLocator).Not.ToBeVisibleAsync();
        }
    }
}
