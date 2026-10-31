# Architecture

**Status:** Draft
**Last reviewed:** 2026-10-31

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

## Confirmed project responsibilities and dependencies

The project responsibilities and compile-time dependency direction are:

| Project | Responsibility | References |
|---|---|---|
| `Web` | ASP.NET Core MVC controllers, views, view models, and application composition in `Program.cs`. | `Application`, `Infrastructure` |
| `Application` | Application services/use cases, DTOs, and application contracts. | `Domain` |
| `Domain` | Domain entities, enums, rules, and exceptions. It has no project references. | — |
| `Infrastructure` | Persistence and integration implementations, including the EF Core context, entity configurations, and migrations. | `Application`, `Domain` |

`Web` references `Infrastructure` to register implementations at the composition root. Application contracts used by infrastructure implementations belong in `Application`; domain-owned abstractions remain in `Domain`. Do not add circular project references. This defines project boundaries and references, not additional business behavior or a requirement to use a specific repository pattern.

## Confirmed architectural principles

- Favor simplicity, low operational cost, easy maintenance, responsiveness, and future evolvability.
- Apply clean-code practices by favoring readable, cohesive, maintainable code and avoiding unnecessary duplication or complexity.
- Use a modular monolith.
- Do not use microservices, Kubernetes, messaging infrastructure, or distributed cache.
- Do not introduce any component that is unnecessary for the application's confirmed low volume of access and transactions.
- An exception or a new architectural component requires an explicit, confirmed requirement and requester approval.

## System overview

- **Primary responsibilities:** To be defined
- **Components and boundaries:** Project responsibilities and compile-time dependencies are defined above; runtime component behavior remains to be defined.
- **Flows between components:** To be defined

## Integrations and dependencies

- External integrations and runtime dependencies: To be defined. Project references are defined above.

## Security and operations

- **Authentication and authorization:** The first delivery uses local username/password authentication, the fixed initial roles defined in [05 — Use Cases](./05-use-cases.md), and the confirmed session, credential, Administrator bootstrap, and account-field policies in [ADR-022](./decisions/022-initial-authentication-policy.md), [ADR-023](./decisions/023-initial-administrator-bootstrap-lifecycle.md), and [ADR-024](./decisions/024-system-user-account-field-rules.md).
- **Secret management:** To be defined
- **Environments and deployment:** To be defined
- **Monitoring and recovery:** To be defined

## Local development environment

The repository must provide:

- `Dockerfile` for the ASP.NET Core application.
- `docker-compose.yml` that configures the ASP.NET Core application and PostgreSQL for local execution.

The confirmed local configuration uses host ports `8080` for the application and `5432` for PostgreSQL, a named volume for PostgreSQL data, and a PostgreSQL healthcheck that the application service waits for before starting. The application applies database migrations during startup. Local credentials are supplied through a Git-ignored `.env` file; `.env.example` lists the required variable names with empty values and contains no credentials. The confirmed names are `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `ConnectionStrings__DefaultConnection`. These settings apply only to local development. See [ADR-021](./decisions/021-docker-local-development-configuration.md).

## Open decisions

- Production hosting and deployment remain to be defined. No production containerization or Docker configuration is implied.

## Decision records

See [decisions/](./decisions/README.md), including [001 — Confirmed technology stack](./decisions/001-confirmed-technology-stack.md), [002 — Confirmed source structure](./decisions/002-confirmed-source-structure.md), [003 — Modular monolith and simplicity principles](./decisions/003-modular-monolith-and-simplicity.md), [004 — Responsive standard layout](./decisions/004-responsive-standard-layout.md), [005 — Docker local development environment](./decisions/005-docker-local-development.md), [020 — Project responsibilities and dependencies](./decisions/020-project-responsibilities-and-dependencies.md), and [021 — Docker local development configuration](./decisions/021-docker-local-development-configuration.md).
