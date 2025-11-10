var builder = DistributedApplication.CreateBuilder(args);

// Add the CMS Admin application
var admin = builder.AddProject<Projects.Dfe_Cms_Web_Admin>("admin")
    .WithHttpsEndpoint(port: 5001, name: "admin-https")
    .WithExternalHttpEndpoints();

// Add the Demo Site application
var demosite = builder.AddProject<Projects.Dfe_Cms_Web_DemoSite>("demosite")
    .WithHttpsEndpoint(port: 5002, name: "demosite-https")
    .WithExternalHttpEndpoints();

builder.Build().Run();
