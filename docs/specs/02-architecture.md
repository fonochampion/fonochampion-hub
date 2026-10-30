# Architecture

**Status:** Draft
**Last reviewed:** 2026-10-29

The technology stack below is confirmed. Do not replace or extend it with alternative technologies without confirmation. Other architectural details remain open until specified.

## Technology decisions

- **Languages and frameworks:** .NET 8 or higher; ASP.NET Core MVC; Entity Framework Core; Bootstrap 5
- **Storage:** PostgreSQL, accessed through Npgsql
- **Hosting and deployment:** To be defined
- **Observability:** To be defined
- **Responsive UI reference:** See [03 — UX and Accessibility](./03-ux-and-accessibility.md) for the confirmed Bootstrap 5 layout and device support.
- **Local development environment:** Docker configuration consisting of a root `Dockerfile` for the ASP.NET Core application and a root `docker-compose.yml` to run the application with PostgreSQL locally. This requirement applies to local development only and does not decide production hosting or deployment.

## Confirmed source structure

Use the following source layout. Do not change its named projects or directories without confirmation:

```text
src/
├── Web/
│   ├── Controllers/
│   ├── Views/
│   ├── Models/
│   ├── Filters/
│   ├── Middleware/
│   ├── Extensions/
│   └── Program.cs
├── Application/
│   ├── Services/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Validators/
│   └── Mappings/
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Interfaces/
│   └── Exceptions/
└── Infrastructure/
    ├── Data/
    │   ├── Context/
    │   ├── Migrations/
    │   └── Configurations/
    ├── Repositories/
    └── Integrations/
```

This specifies the approved directory layout only; responsibilities and dependency rules not stated here remain open.

## Confirmed architectural principles

- Favor simplicity, low operational cost, easy maintenance, responsiveness, and future evolvability.
- Use a modular monolith.
- Do not use microservices, Kubernetes, messaging infrastructure, or distributed cache.
- Do not introduce any component that is unnecessary for the application's confirmed low volume of access and transactions.
- An exception or a new architectural component requires an explicit, confirmed requirement and requester approval.

## System overview

- **Primary responsibilities:** To be defined
- **Components and boundaries:** To be defined
- **Flows between components:** To be defined

## Integrations and dependencies

- To be defined

## Security and operations

- **Authentication and authorization:** The first delivery uses local username/password authentication and the fixed initial roles defined in [05 — Use Cases](./05-use-cases.md). Implementation-level session, credential-policy, and bootstrap-lifecycle details remain to be defined.
- **Secret management:** To be defined
- **Environments and deployment:** To be defined
- **Monitoring and recovery:** To be defined

## Local development environment

The repository must provide:

- `Dockerfile` for the ASP.NET Core application.
- `docker-compose.yml` that configures the ASP.NET Core application and PostgreSQL for local execution.

The configuration must be ready for local use. Port mappings, local credentials, environment-variable names, data persistence, initialization, and startup/readiness behavior remain unspecified and must not be invented as confirmed requirements.

## Open decisions

- Docker local configuration details listed above remain to be defined.

## Decision records

See [decisions/](./decisions/README.md), including [001 — Confirmed technology stack](./decisions/001-confirmed-technology-stack.md), [002 — Confirmed source structure](./decisions/002-confirmed-source-structure.md), [003 — Modular monolith and simplicity principles](./decisions/003-modular-monolith-and-simplicity.md), [004 — Responsive standard layout](./decisions/004-responsive-standard-layout.md), and [005 — Docker local development environment](./decisions/005-docker-local-development.md).
