# ADR-001: Confirmed Technology Stack

**Status:** Accepted
**Date:** 2026-10-29

## Context

The project needs a confirmed technology baseline for agents and implementation planning. The requester specified the stack.

## Options considered

1. Use the technology stack specified by the requester.
2. Leave the technology stack undecided.

## Decision

Use the following stack:

- .NET 8 or higher
- ASP.NET Core MVC
- Entity Framework Core
- PostgreSQL
- Bootstrap 5
- Npgsql

Npgsql is the PostgreSQL data provider. Hosting, deployment, observability, and other architecture details remain unspecified.

## Consequences

- **Benefits:** Agents have a confirmed technology baseline and can avoid proposing conflicting technologies.
- **Costs or risks:** Versions and compatibility among framework/provider packages must be selected consistently during implementation.
- **Follow-up actions:** Update implementation plans and agent instructions to consult this decision. Do not infer hosting, deployment, or other unspecified choices from this stack.
