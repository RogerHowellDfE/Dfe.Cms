using System.Net;

namespace Dfe.Cms.Web.Admin.Tests;

/// <summary>
/// Validates security headers to ensure OWASP compliance and prevent information disclosure.
/// Reference: OWASP Secure Headers Project
/// https://owasp.org/www-project-secure-headers/
/// </summary>
public class SecurityHeadersTests : IClassFixture<AdminWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SecurityHeadersTests(AdminWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ========================================
    // Information Disclosure Prevention
    // ========================================

    /// <summary>
    /// Validates that the Server header is not exposed.
    /// Per OWASP: Server headers can reveal technology stack information.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_DoesNotExposeServerHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.False(response.Headers.Contains("Server"));
    }

    [Fact]
    public async Task HomePage_WhenRequested_DoesNotExposeXPoweredByHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.False(response.Headers.Contains("X-Powered-By"));
    }

    [Fact]
    public async Task HomePage_WhenRequested_DoesNotExposeXAspNetVersionHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.False(response.Headers.Contains("X-AspNet-Version"));
    }

    [Fact]
    public async Task HomePage_WhenRequested_DoesNotExposeXAspNetMvcVersionHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.False(response.Headers.Contains("X-AspNetMvc-Version"));
    }

    // ========================================
    // Security Headers - Basic Protection
    // ========================================

    /// <summary>
    /// Validates X-Content-Type-Options header prevents MIME-sniffing.
    /// Per OWASP: Prevents browsers from interpreting files as a different MIME type.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_IncludesXContentTypeOptionsHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());
    }

    /// <summary>
    /// Validates X-Frame-Options header prevents clickjacking.
    /// Per OWASP: Prevents the page from being embedded in iframes.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_IncludesXFrameOptionsHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("X-Frame-Options"));
        Assert.Equal("deny", response.Headers.GetValues("X-Frame-Options").First());
    }

    /// <summary>
    /// Validates Content-Security-Policy header restricts resource loading.
    /// Per OWASP: Mitigates XSS and data injection attacks.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_IncludesContentSecurityPolicyHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Content-Security-Policy"));
        var csp = response.Headers.GetValues("Content-Security-Policy").First();
        Assert.Contains("default-src 'self'", csp);
    }

    /// <summary>
    /// Validates Strict-Transport-Security header enforces HTTPS.
    /// Per OWASP: Forces browsers to use HTTPS for all future requests.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_IncludesStrictTransportSecurityHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Strict-Transport-Security"));
        var hsts = response.Headers.GetValues("Strict-Transport-Security").First();
        Assert.Contains("max-age=", hsts);
    }

    /// <summary>
    /// Validates Referrer-Policy header controls referrer information.
    /// Per OWASP: Prevents leaking sensitive URLs via referrer.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_IncludesReferrerPolicyHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Referrer-Policy"));
        Assert.Equal("no-referrer", response.Headers.GetValues("Referrer-Policy").First());
    }

    /// <summary>
    /// Validates Permissions-Policy header restricts browser features.
    /// Per OWASP: Controls which browser features can be used.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_IncludesPermissionsPolicyHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Permissions-Policy"));
    }

    // ========================================
    // Security Headers - Additional Protection
    // ========================================

    [Fact]
    public async Task HomePage_WhenRequested_IncludesXDnsPrefetchControlHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("X-DNS-Prefetch-Control"));
        Assert.Equal("off", response.Headers.GetValues("X-DNS-Prefetch-Control").First());
    }

    [Fact]
    public async Task HomePage_WhenRequested_IncludesXPermittedCrossDomainPoliciesHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("X-Permitted-Cross-Domain-Policies"));
        Assert.Equal("none", response.Headers.GetValues("X-Permitted-Cross-Domain-Policies").First());
    }

    [Fact]
    public async Task HomePage_WhenRequested_IncludesCrossOriginEmbedderPolicyHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Cross-Origin-Embedder-Policy"));
        Assert.Equal("require-corp", response.Headers.GetValues("Cross-Origin-Embedder-Policy").First());
    }

    [Fact]
    public async Task HomePage_WhenRequested_IncludesCrossOriginOpenerPolicyHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Cross-Origin-Opener-Policy"));
        Assert.Equal("same-origin", response.Headers.GetValues("Cross-Origin-Opener-Policy").First());
    }

    [Fact]
    public async Task HomePage_WhenRequested_IncludesCrossOriginResourcePolicyHeader()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Cross-Origin-Resource-Policy"));
        Assert.Equal("same-origin", response.Headers.GetValues("Cross-Origin-Resource-Policy").First());
    }

    // ========================================
    // Cache Control
    // ========================================

    /// <summary>
    /// Validates Cache-Control header follows OWASP recommendations.
    /// Per OWASP: Prevents caching of sensitive content.
    /// </summary>
    [Fact]
    public async Task HomePage_WhenRequested_CacheControlMatchesOwaspRecommendation()
    {
        var response = await _client.GetAsync("/");

        var cacheControl = response.Headers.CacheControl;
        Assert.NotNull(cacheControl);
        Assert.True(cacheControl.NoStore);
        Assert.Equal(0, cacheControl.MaxAge?.TotalSeconds);
    }

    // ========================================
    // Consistency Checks
    // ========================================

    [Fact]
    public async Task StaticAsset_WhenRequested_DoesNotExposeServerHeader()
    {
        var response = await _client.GetAsync("/css/site.css");

        Assert.False(response.Headers.Contains("Server"));
    }

    [Fact]
    public async Task HomePage_WhenRequestedViaPost_DoesNotExposeServerHeader()
    {
        var response = await _client.PostAsync("/", null);

        Assert.False(response.Headers.Contains("Server"));
    }

    [Fact]
    public async Task HomePage_WhenRequestedMultipleTimes_ConsistentlyRemovesServerHeaders()
    {
        for (int i = 0; i < 3; i++)
        {
            var response = await _client.GetAsync("/");
            Assert.False(response.Headers.Contains("Server"));
            Assert.False(response.Headers.Contains("X-Powered-By"));
        }
    }

    [Fact]
    public async Task ErrorResponse_WhenTriggered_DoesNotExposeServerHeader()
    {
        var response = await _client.GetAsync("/this-page-does-not-exist");

        Assert.False(response.Headers.Contains("Server"));
    }
}
