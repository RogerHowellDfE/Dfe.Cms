using System.Net;
using System.Text.RegularExpressions;
using Xunit;

namespace Dfe.Cms.Web.DemoSite.Tests;

/// <summary>
/// Tests validating compliance with GOV.UK and DfE design systems.
/// Per: https://design.education.gov.uk/design-system/dfe-frontend/install
/// Per: https://design.education.gov.uk/design-system/govuk-rebrand/dfe-header-rebrand
///
/// These tests verify:
/// 1. Compiled CSS and JS contain required code from GOV.UK/DfE Frontend
/// 2. HTML structure uses correct classes, components, and assets
/// </summary>
public class DesignSystemComplianceTests : IClassFixture<PublicWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DesignSystemComplianceTests(PublicWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ========================================
    // Compiled CSS validation
    // ========================================

    /// <summary>
    /// Validates that the compiled CSS contains the 1200px width container override.
    /// Per: https://design.education.gov.uk/design-system/dfe-frontend/install
    /// </summary>
    [Fact]
    public async Task SiteCss_WhenRequested_Contains1200pxWidthContainer()
    {
        var response = await _client.GetAsync("/css/site.css");
        var css = await response.Content.ReadAsStringAsync();

        var has1200pxWidth = Regex.IsMatch(css, @"\.govuk-width-container\s*\{[^}]*max-width\s*:\s*1200px", RegexOptions.IgnoreCase);

        Assert.True(has1200pxWidth, "Compiled CSS should contain .govuk-width-container with max-width: 1200px");
    }

    /// <summary>
    /// Validates that the compiled CSS contains the DfE rebrand logo height styling.
    /// Per: https://design.education.gov.uk/design-system/govuk-rebrand/dfe-header-rebrand
    /// </summary>
    [Fact]
    public async Task SiteCss_WhenRequested_ContainsDfeLogoHeight()
    {
        var response = await _client.GetAsync("/css/site.css");
        var css = await response.Content.ReadAsStringAsync();

        // Check for the logo height rule (may be minified)
        var hasLogoHeight = Regex.IsMatch(css, @"\.govuk-header__link\s+\.govuk-header__logotype\s*\{[^}]*height\s*:\s*30px", RegexOptions.IgnoreCase);

        Assert.True(hasLogoHeight, "Compiled CSS should contain .govuk-header__link .govuk-header__logotype with height: 30px");
    }

    // ========================================
    // Compiled JavaScript validation
    // ========================================

    /// <summary>
    /// Validates that GOV.UK Frontend JavaScript is bundled in site.js.
    /// Per: https://design.education.gov.uk/design-system/dfe-frontend/install
    /// </summary>
    [Fact]
    public async Task SiteJs_WhenRequested_ContainsGovukFrontend()
    {
        var response = await _client.GetAsync("/js/site.js");
        var js = await response.Content.ReadAsStringAsync();

        var containsGovukCode = js.Contains("govuk-frontend") || js.Contains("initAll") || js.Contains("GOVUKFrontend");

        Assert.True(containsGovukCode, "site.js should contain GOV.UK Frontend JavaScript");
    }

    // ========================================
    // HTML structure validation - GOV.UK Rebrand
    // ========================================

    /// <summary>
    /// Validates that the HTML element has the rebrand class.
    /// Per: https://design.education.gov.uk/design-system/govuk-rebrand/dfe-header-rebrand
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_HtmlElementHasRebrandClass()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("govuk-template--rebranded", content);
    }

    /// <summary>
    /// Validates that the page uses govuk-header class.
    /// Per: https://design.education.gov.uk/design-system/govuk-rebrand/dfe-header-rebrand
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_UsesGovukHeaderClass()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("govuk-header", content);
    }

    /// <summary>
    /// Validates that the page does not use the legacy dfe-header class.
    /// Older versions of dfe-frontend used dfe-header, but current versions override govuk-header instead.
    /// Per: https://design.education.gov.uk/design-system/govuk-rebrand/dfe-header-rebrand
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_DoesNotUseLegacyDfeHeaderClass()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("dfe-header", content);
    }

    /// <summary>
    /// Validates that the page uses govuk-footer class.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_UsesGovukFooterClass()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("govuk-footer", content);
    }

    /// <summary>
    /// Validates that the page does not use the legacy dfe-footer class.
    /// Older versions of dfe-frontend used dfe-footer, but current versions override govuk-footer instead.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_DoesNotUseLegacyDfeFooterClass()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("dfe-footer", content);
    }

    /// <summary>
    /// Validates that the Service Navigation component is present.
    /// Per: https://design.education.gov.uk/design-system/govuk-rebrand/dfe-header-rebrand
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_IncludesServiceNavigationComponent()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("govuk-service-navigation", content);
        Assert.Contains("govuk-service-navigation__service-name", content);
    }

    /// <summary>
    /// Validates that the logo has correct alt text.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_LogoHasCorrectAltText()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("alt=\"Department for Education\"", content);
    }

    /// <summary>
    /// Validates that the HTML references the correct DfE rebrand logo filename.
    /// Per: https://design.education.gov.uk/design-system/govuk-rebrand/dfe-header-rebrand
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_ReferencesDfeRebrandLogoInHtml()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Should contain reference to DfE rebrand logo (with or without version hash)
        // Pattern matches: /assets/images/department-for-education_white.png or with hash like .abc123.png
        // Note: .NET 9's MapStaticAssets() (see Dfe.Cms.Web.DemoSite.Program) automatically adds
        // content-addressable hashes to all static assets, even without asp-append-version="true"
        var hasRebrandLogo = Regex.IsMatch(content, @"/assets/images/department-for-education_white(\.[a-z0-9]+)?\.png", RegexOptions.IgnoreCase);

        Assert.True(hasRebrandLogo, "HTML should contain reference to DfE rebrand logo (department-for-education_white.png with optional version hash)");
    }

    // ========================================
    // HTML structure validation - DfE Frontend
    // ========================================

    /// <summary>
    /// Validates that the HTML uses the govuk-width-container class.
    /// Per: https://design.education.gov.uk/design-system/dfe-frontend/install
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_UsesGovukWidthContainerClass()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("govuk-width-container", html);
    }

    // ========================================
    // Page title validation
    // ========================================

    /// <summary>
    /// Validates that the page title follows the GOV.UK pattern of ending with " - GOV.UK".
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_TitleEndsWithGovUkSuffix()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        var titleMatch = Regex.Match(html, @"<title>(.+?)</title>", RegexOptions.IgnoreCase);
        Assert.True(titleMatch.Success, "Page should have a <title> tag");

        var title = titleMatch.Groups[1].Value;
        Assert.EndsWith(" - GOV.UK", title);
    }

    /// <summary>
    /// Validates that the home page title ends with the service name and GOV.UK suffix.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_TitleEndsWithServiceNameAndGovUk()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        var titleMatch = Regex.Match(html, @"<title>(.+?)</title>", RegexOptions.IgnoreCase);
        Assert.True(titleMatch.Success, "Page should have a <title> tag");

        var title = titleMatch.Groups[1].Value;
        Assert.EndsWith("Demo Service - GOV.UK", title);
    }
}
