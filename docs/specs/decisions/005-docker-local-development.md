# ADR-005: Docker Local Development Environment

**Status:** Accepted
**Date:** 2026-10-29

## Context

The requester requires a ready-to-run local development environment for the ASP.NET Core application and PostgreSQL using Docker.

## Options considered

1. Provide Docker configuration for local development.
2. Leave local environment setup unspecified.

## Decision

Provide a root `Dockerfile` for the ASP.NET Core application and a root `docker-compose.yml` that configures the application and PostgreSQL to run locally.

This decision applies only to local development. It does not decide production containerization, hosting, or deployment. Port mappings, local credentials, environment-variable names, data persistence, initialization, and startup/readiness behavior remain open.

## Consequences

- **Benefits:** Developers have a consistent, ready-to-run local environment for the application and database.
- **Costs or risks:** Docker and Docker Compose are required for this local workflow; configuration details must be selected without exposing production secrets.
- **Follow-up actions:** When implementing the environment, consult [02 — Architecture](../02-architecture.md#local-development-environment) and resolve open configuration details as needed. This specification change does not create the Docker files.
