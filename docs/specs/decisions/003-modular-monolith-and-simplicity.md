# ADR-003: Modular Monolith and Simplicity Principles

**Status:** Accepted
**Date:** 2026-10-29

## Context

The application is expected to have low volume of access and transactions. The requester prioritized simplicity, low operational cost, maintainability, responsiveness, and future evolution, and ruled out unnecessary distributed architecture.

## Options considered

1. Use a simple modular monolith, adding components only for confirmed needs.
2. Introduce distributed architecture or additional infrastructure preemptively.

## Decision

Apply these principles:

- Favor simplicity, low operational cost, easy maintenance, responsiveness, and future evolvability.
- Use a modular monolith.
- Do not use microservices, Kubernetes, messaging infrastructure, or distributed cache.
- Do not introduce components unnecessary for the confirmed low volume of access and transactions.
- Require an explicit, confirmed requirement and requester approval before making an exception or adding an architectural component.

The project structure and technology baseline remain as documented in [02 — Architecture](../02-architecture.md).

## Consequences

- **Benefits:** Limits operational complexity and cost while preserving a modular structure that can evolve.
- **Costs or risks:** Any future need that conflicts with these constraints must be explicitly evaluated and approved rather than assumed.
- **Follow-up actions:** Agents must justify proposed components against confirmed requirements and retain the approved architecture unless the requester changes it.
