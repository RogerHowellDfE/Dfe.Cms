# DfE CMS (Content Management System)

This project is an implementation of a headless CMS for the Department for Education.

Future intent is to provide tag helpers to embed content within your applications, allowing you to provide compile-time information about which content is dynamically loaded from the CMS, and to allow for customisation of how it is rendered.

This will, for example, allow for custom borders to be applied, enabling non-technical users to see which parts are fixed vs dynamic. It will also allow for in-context editing of content directly within the application.


## Quick Start

We optimise for developer experience, providing an excellent "first run" experience with minimal manual steps involved.

Additional steps will only be required as part of the deployment to use "real" non-test/fake implementations for infrastructure.
For example, if you wish to override the database connection to use an external database when running locally, or to configure authentication providers.

```powershell
# Build (automatically installs NPM dependencies and compiles assets)
dotnet build

# Run tests
dotnet test

# Run admin application (https://localhost:5001)
dotnet run --project src/Dfe.Cms.Web.Admin

# Run public application (https://localhost:5002)
dotnet run --project src/Dfe.Cms.Web.DemoSite
```

