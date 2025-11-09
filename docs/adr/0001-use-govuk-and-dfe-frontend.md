# ADR-0001: Use GOV.UK Frontend and DfE Frontend for UI

**Status:** Accepted

**Date:** 2025-11-09

## Context

All DfE digital services must follow the GOV.UK Design System unless there is a strong/compelling reason to do otherwise (e.g. branded campaign sites).


## Decision

We will use:
- **GOV.UK Frontend v5.x** for core components, patterns, and base styling
  - Latest available version
  - Supplied via the C#/dotnet dependency [x-govuk `GovUk.Frontend.AspNetCore`](https://github.com/x-govuk/govuk-frontend-aspnetcore) (as opposed to importing the npm package)
- **DfE Frontend v2.x** for DfE-specific branding and overrides
  - Latest available version
  - Supplied via the npm package: `dfe-frontend`
- **Rebranded styling**
  - GOV.UK recently did a rebranding
  - Existing services will need to migrate
  - This new project will start straight away on the new / rebranded styling

