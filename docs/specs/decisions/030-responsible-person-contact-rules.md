# ADR-030: Responsible-Person Contact Rules

**Status:** Accepted
**Date:** 2026-11-11

## Context

Contact requiredness, multiplicity, preferred-contact behavior, and validation were open.

## Options considered

1. Require both telephone and email, one of each.
2. Require at least one of telephone or email and allow multiple of either.
3. Make both optional.
4. Accept Brazilian telephone numbers or require an international format.

## Decision

At least one telephone or email contact is required. Multiple values of either type are allowed. No preferred contact is defined. A telephone number has a Brazilian DDD and subscriber number (10 or 11 digits); the `+55` country prefix is optional, and spaces, parentheses, and hyphens may be used as separators. Email addresses must pass syntax validation. Contact values are trimmed at their boundaries, are not otherwise normalized, and are not verified for ownership.

## Consequences

- **Benefits:** Allows contact through either channel while supporting multiple numbers or addresses and accepts the confirmed Brazilian number formats.
- **Costs or risks:** Users must provide at least one contact; phone and email values are validated syntactically but are not verified for ownership.
- **Follow-up actions:** None.
