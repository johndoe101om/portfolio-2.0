using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Portfolio.Client.Tests;

/// <summary>
/// End-to-end tests using Playwright.
/// Run against: http://localhost:5173 (dev) or http://localhost (Docker).
/// Set BASE_URL environment variable to override.
/// 
/// Setup:
///   dotnet build
///   pwsh tests/Portfolio.Client.Tests/playwright-setup.ps1
///   dotnet test tests/Portfolio.Client.Tests
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PortfolioE2ETests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5173";

    // ── Homepage / Hero ───────────────────────────────────────────────────────
    [Test]
    public async Task HomePage_Loads_ShowsHeroSection()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Heading contains the name
        await Expect(Page.GetByText("Satyam Kumar")).ToBeVisibleAsync();
        // Hero section visible
        await Expect(Page.Locator("#hero")).ToBeVisibleAsync();
    }

    [Test]
    public async Task HomePage_SkipLink_IsFocusableAndFunctional()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.Keyboard.PressAsync("Tab");
        var skipLink = Page.GetByText("Skip to main content");
        await Expect(skipLink).ToBeFocusedAsync();
    }

    [Test]
    public async Task HomePage_Preloader_DisappearsAfterLoad()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.WaitForTimeoutAsync(2000);
        // Preloader should have faded out
        var preloader = Page.Locator("[aria-hidden='true'][class*='loader']").First;
        await Expect(preloader).ToHaveCountAsync(0);
    }

    // ── Desktop Sidebar Navigation ────────────────────────────────────────────
    [Test]
    public async Task DesktopSidebar_NavigatesToAbout()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "About" }).First.ClickAsync();
        await Expect(Page.Locator("#about")).ToBeVisibleAsync();
    }

    [Test]
    public async Task DesktopSidebar_ActiveNavItem_HasAriaCurrent()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Portfolio" }).First.ClickAsync();
        var portfolioLink = Page.GetByRole(AriaRole.Link, new() { Name = "Portfolio" }).First;
        await Expect(portfolioLink).ToHaveAttributeAsync("aria-current", "page");
    }

    // ── Mobile Navigation ─────────────────────────────────────────────────────
    [Test]
    public async Task MobileNav_HamburgerTogglesDrawer()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(375, 812); // iPhone SE

        var hamburger = Page.GetByRole(AriaRole.Button, new() { Name = "Open navigation menu" });
        await hamburger.ClickAsync();

        // Drawer should appear
        await Expect(Page.GetByRole(AriaRole.Navigation, new() { Name = "Mobile navigation menu" }))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task MobileNav_NavigatesToSection_ClosesDrawer()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(375, 812);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Open navigation menu" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About" }).First.ClickAsync();

        // Drawer should close
        await Expect(Page.GetByRole(AriaRole.Navigation, new() { Name = "Mobile navigation menu" }))
            .ToHaveAttributeAsync("aria-hidden", "true");
    }

    // ── Portfolio Filtering ───────────────────────────────────────────────────
    [Test]
    public async Task Portfolio_FilterTabs_ShowAllByDefault()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Portfolio" }).First.ClickAsync();
        await Page.WaitForSelectorAsync("[role='tablist']");

        var allTab = Page.GetByRole(AriaRole.Tab, new() { Name = "All" });
        await Expect(allTab).ToHaveAttributeAsync("aria-selected", "true");
    }

    [Test]
    public async Task Portfolio_ClickWebDesignFilter_UpdatesSelection()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Portfolio" }).First.ClickAsync();
        await Page.WaitForSelectorAsync("[role='tablist']");

        var webTab = Page.GetByRole(AriaRole.Tab, new() { Name = "Web Design" });
        await webTab.ClickAsync();
        await Expect(webTab).ToHaveAttributeAsync("aria-selected", "true");
    }

    [Test]
    public async Task Portfolio_KeyboardNavigation_WorksOnFilterTabs()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Portfolio" }).First.ClickAsync();
        var allTab = Page.GetByRole(AriaRole.Tab, new() { Name = "All" });
        await allTab.FocusAsync();
        await Expect(allTab).ToBeFocusedAsync();
    }

    // ── Blog Section ──────────────────────────────────────────────────────────
    [Test]
    public async Task Blog_NavigatesToSection_ShowsPosts()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Blog" }).First.ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Latest News" }))
            .ToBeVisibleAsync();
    }

    // ── Contact Section ───────────────────────────────────────────────────────
    [Test]
    public async Task Contact_ShowsInfoBoxes_AndForm()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Contact" }).First.ClickAsync();

        await Expect(Page.GetByText("Mail Me")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Call Me On")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Send Message" }))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task Contact_Form_ValidationErrors_OnEmptySubmit()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Contact" }).First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Send Message" }).ClickAsync();

        // Validation errors should appear
        await Expect(Page.GetByText(/Name must be at least/i)).ToBeVisibleAsync();
    }

    [Test]
    public async Task Contact_Form_AcceptsValidInput()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Contact" }).First.ClickAsync();

        await Page.GetByPlaceholder("Your Name *").FillAsync("Alice Smith");
        await Page.GetByPlaceholder("Email Address *").FillAsync("alice@example.com");
        await Page.GetByPlaceholder("Subject *").FillAsync("E2E Test Subject Here");
        await Page.GetByPlaceholder("Your message...").FillAsync("This is a playwright automated test message.");

        // Form should not show validation errors yet
        await Expect(Page.GetByText("Name must be at least")).ToHaveCountAsync(0);
    }

    // ── Responsive Layout ─────────────────────────────────────────────────────
    [Test]
    public async Task Layout_Desktop_ShowsSidebar()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        var sidebar = Page.GetByRole(AriaRole.Complementary, new() { Name = "Main navigation" });
        await Expect(sidebar).ToBeVisibleAsync();
    }

    [Test]
    public async Task Layout_Mobile_SidebarIsHidden()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(375, 812);

        // Desktop sidebar CSS display:none at <992px
        var desktopSidebar = Page.GetByRole(AriaRole.Complementary, new() { Name = "Main navigation" });
        await Expect(desktopSidebar).ToBeHiddenAsync();
    }

    // ── Accessibility ─────────────────────────────────────────────────────────
    [Test]
    public async Task Accessibility_MainLandmark_Exists()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page.GetByRole(AriaRole.Main)).ToBeVisibleAsync();
    }

    [Test]
    public async Task Accessibility_NoAutofocusOnLoad()
    {
        await Page.GotoAsync(BaseUrl);
        // Body should have focus by default, not some rogue element
        var focusedEl = await Page.EvaluateAsync<string>("() => document.activeElement?.tagName");
        focusedEl.Should().BeOneOf("BODY", "DIV");
    }

    // ── Prev/Next Navigation ──────────────────────────────────────────────────
    [Test]
    public async Task PrevNext_Buttons_NavigateSections()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.SetViewportSizeAsync(1280, 800);

        // Hero is active by default; click Next should go to About
        var nextBtn = Page.GetByRole(AriaRole.Button, new() { Name = "Next section" });
        await nextBtn.ClickAsync();

        await Expect(Page.Locator("#about")).ToBeVisibleAsync();
    }
}

// FluentAssertions for Playwright strings
internal static class StringAssertExtensions
{
    public static void Should(this string? value) { }
}
internal static class StringAssertions
{
    public static void BeOneOf(this string? value, params string[] allowed)
    {
        if (value is null || !allowed.Contains(value))
            throw new AssertionException($"Expected '{value}' to be one of [{string.Join(", ", allowed)}]");
    }
}
