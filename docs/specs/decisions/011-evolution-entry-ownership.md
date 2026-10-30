# ADR-011: Evolution Entry Ownership

**Status:** Accepted
**Date:** 2026-10-30

## Context

Each participating professional has a separate evolution entry for an attendance. The prior specification did not clarify whether one professional could edit another participant's entry.

## Options considered

1. Allow each participating professional to maintain only their own evolution entry.
2. Allow any participating professional to maintain any participant's evolution entry.

## Decision

Each professional participating in an attendance may record or update only their own evolution entry. Administrators may access clinical data but cannot change another professional's evolution; other role access is specified in ADR-018.

## Consequences

- **Benefits:** Evolution entries remain attributable to the participating professional responsible for them.
- **Costs or risks:** Data privacy, legal requirements, and account/authentication details remain open.
- **Follow-up actions:** Apply the role permissions in ADR-018 and resolve remaining privacy and authentication requirements before implementation.
