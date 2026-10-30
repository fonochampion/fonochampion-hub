# ADR-016: Retain Associations During Professional Inactivation

**Status:** Accepted
**Date:** 2026-10-30

## Context

Professionals may be inactivated while associated with patients. The effect of inactivation on these associations and scheduling eligibility requires a defined rule.

## Options considered

1. Remove patient associations automatically when a professional is inactivated.
2. Retain the associations, prevent new scheduling involving the inactive professional, and restore eligibility when the professional is reactivated.

## Decision

Professional inactivation retains existing patient associations. While inactive, the professional cannot be used for new scheduling. Reactivation restores the retained associations automatically. Inactivation does not alter existing attendance participation or prevent the professional from participating and recording/updating their own evolution under the normal attendance-status rules. Rescheduling removes inactive participants and applies the replacement-selection flow in ADR-017.

This decision resolves the association and reactivation aspects left open by ADR-008.

## Consequences

- **Benefits:** Reactivation does not require rebuilding patient associations, while inactive professionals are excluded from new scheduling.
- **Costs or risks:** Professional deletion semantics remain open.
- **Follow-up actions:** Define professional deletion semantics before implementing that part of the lifecycle. Scheduling permissions are specified in ADR-018.
