# ADR-030: Responsible-Person Contact Rules

**Status:** Accepted
**Date:** 2026-11-11

## Context

Contact requiredness, multiplicity, preferred-contact behavior, and validation were open.

## Options considered

1. Require both telephone and email, one of each.
2. Require at least one of telephone or email and allow multiple of either.
3. Make both optional.

## Decision

At least one telephone or email contact is required. Multiple values of either type are allowed. No preferred contact is defined. Contact values must pass format validation.

## Consequences

- **Benefits:** Allows contact through either channel while supporting multiple numbers or addresses.
- **Costs or risks:** Users must provide at least one contact; validation rules must be applied consistently.
- **Follow-up actions:** None.
