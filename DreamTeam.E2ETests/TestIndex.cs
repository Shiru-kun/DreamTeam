using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace DreamTeam.E2ETests
{

    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : PageTest
    {
        public readonly static string HOST_URL = "https://google.com";
        [Test]
        public async Task Should_Render_Input_Search_Enabled_And_Type_ABC()
        {
          
                // Arrange
                await Page.GotoAsync($"{HOST_URL}");

                //Act 
                await Page.GetByLabel("Pesq.").FillAsync("ABC");

                //Assertion
                await Expect(Page.GetByLabel("Pesq.")).ToHaveValueAsync("ABC");
        }
    }
}