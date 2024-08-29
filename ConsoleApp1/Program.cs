using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false,
});
var context = await browser.NewContextAsync();

var page = await context.NewPageAsync();
await page.GotoAsync("");
await page.GetByRole(AriaRole.Banner).GetByRole(AriaRole.Button, new() { Name = "Iniciar Sessão" }).ClickAsync();
await page.Locator("input[name=\"login\"]").ClickAsync();
await page.GetByRole(AriaRole.Button, new() { Name = "Mais Tarde" }).ClickAsync();
await page.Locator("input[name=\"login\"]").ClickAsync();
await page.Locator("input[name=\"login\"]").FillAsync("");
await page.Locator("input[name=\"password\"]").ClickAsync();
await page.Locator("input[name=\"password\"]").FillAsync("");
await page.Locator("button").Filter(new() { HasText = "Iniciar Sessão de forma segura" }).ClickAsync();
await page.GetByRole(AriaRole.Link, new() { Name = "Aviator", Exact = true }).ClickAsync();
await page.FrameLocator("iframe[title=\"Aviator\"]").FrameLocator("#spribe-game").GetByRole(AriaRole.Button, new() { Name = "Aposta 8.00 MZN" }).First.ClickAsync();
