using Dfe.Cms.Web.DemoSite.Middleware;
using GovUk.Frontend.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (telemetry, health checks, service discovery)
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();

// Add Gov.UK Frontend services with 2025 rebrand
builder.Services.AddGovUkFrontend(options =>
{
    options.Rebrand = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Security headers middleware must be first to ensure headers are set for all responses
app.UseSecurityHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Map Aspire default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

app.Run();

// Make Program accessible to WebApplicationFactory for testing
public partial class Program { }
