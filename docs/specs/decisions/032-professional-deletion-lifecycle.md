# ADR-032: Professional Deletion Lifecycle

**Status:** Accepted
**Date:** 2026-11-11

## Context

Professional deletion is allowed only when no patient is associated, but its semantics were open. The product also has a separate reversible inactivation operation.

## Options considered

1. Physically delete the professional when no patient is associated.
2. Treat deletion as reversible inactivation.
3. Use a terminal logical-deletion state distinct from inactivation.

## Decision

Deletion is logical and terminal, distinct from reversible inactivation. It is allowed only when no patient is associated, preserves historical references, and cannot be reversed by reactivation.

## Consequences

- **Benefits:** Preserves historical references and keeps deletion distinct from temporary inactivation.
- **Costs or risks:** Deleted records remain stored and need clear exclusion from active workflows.
- **Follow-up actions:** None.
