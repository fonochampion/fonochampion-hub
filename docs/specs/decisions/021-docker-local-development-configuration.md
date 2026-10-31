# ADR-021: Docker Local Development Configuration

**Status:** Accepted
**Date:** 2026-10-31

## Context

ADR-005 confirms a root `Dockerfile` and `docker-compose.yml` for local ASP.NET Core and PostgreSQL development, while leaving implementation details open. IMP-003 needs a specific local configuration to be ready for implementation.

## Options considered

1. Select concrete local ports, persistence, credential input, and database startup behavior.
2. Leave the local Docker configuration details open.

## Decision

- Map the ASP.NET Core application to host port `8080` and PostgreSQL to host port `5432`.
- Persist PostgreSQL data in a named Docker volume.
- Require a PostgreSQL healthcheck and configure the application service to wait until PostgreSQL is healthy before starting.
- Apply database migrations during application startup.
- Supply local credentials through a Git-ignored `.env` file. Provide a `.env.example` containing the required variable names with empty values only; do not commit credentials.
- Use the environment-variable names `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `ConnectionStrings__DefaultConnection`.
- These decisions apply only to local development and do not define production containerization, hosting, or deployment.

## Consequences

- **Benefits:** Defines a repeatable local environment, preserves the local database across container recreation, and avoids committing credential values.
- **Costs or risks:** Developers must populate `.env` locally. Application startup depends on PostgreSQL readiness and database migrations completing successfully.
- **Follow-up actions:** Implement IMP-003 using these settings and the confirmed .NET/project structure. No Docker files are created by this decision record.
