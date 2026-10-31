# ADR-020: Project Responsibilities and Dependencies

**Status:** Accepted
**Date:** 2026-10-31

## Context

The approved source layout names four projects but did not define their responsibilities or compile-time dependency direction. The persistence setup in IMP-002 requires these boundaries to be explicit.

## Options considered

1. Define simple responsibilities and one-way dependencies across the existing four projects.
2. Leave project responsibilities and dependencies open.

## Decision

- `Web` contains ASP.NET Core MVC controllers, views, view models, and the application composition root in `Program.cs`.
- `Application` contains application services/use cases, DTOs, and application contracts.
- `Domain` contains domain entities, enums, rules, and exceptions, and has no project references.
- `Infrastructure` contains persistence and integration implementations, including the EF Core context, entity configurations, and migrations.
- Project references are `Web → Application`, `Web → Infrastructure`, `Application → Domain`, and `Infrastructure → Application` plus `Domain`. No circular project references are allowed.
- `Web` references `Infrastructure` to register implementations at the composition root. Application contracts implemented by Infrastructure belong in `Application`; abstractions owned by the domain remain in `Domain`.
- These boundaries do not require a repository pattern or define additional business behavior.

## Consequences

- **Benefits:** Defines a clear, one-way project structure for configuring MVC and persistence while keeping the domain independent of infrastructure.
- **Costs or risks:** `Web` has a compile-time reference to `Infrastructure` for composition; avoid using that reference outside application startup and registration.
- **Follow-up actions:** Configure IMP-002 using these project boundaries and the confirmed EF Core/Npgsql/PostgreSQL stack.
