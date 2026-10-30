# ADR-008: Patient-Professional Association

**Status:** Accepted
**Date:** 2026-10-29

## Context

Patient registration must allow the system user to include the professional or professionals who will attend the patient. The relationship also supports the existing rule that a professional associated with a patient cannot be deleted.

## Options considered

1. Require a professional association when creating a patient.
2. Allow patient registration with no professional association and let the user select registered professionals.

## Decision

- Professional association during patient registration is optional. A patient may be registered with no associated professional.
- During patient registration, the user may select one or more already registered and active professionals.
- An active, registered professional may be added later through the patient record.
- A professional may be associated with multiple patients.
- An unregistered or inactive professional cannot be selected for a new association during patient registration.
- Each attendance must have one or more active professionals associated with the patient when scheduling completes. If none are actively associated, the scheduling flow creates associations for the selected active professionals, as specified by ADR-017.
- A professional associated with one or more patients cannot be deleted.
- An association may be removed without deleting or altering existing attendance/evolution records, and already scheduled attendances and their participants remain unchanged, as specified by ADR-015.
- Professional inactivation retains existing patient associations, prevents new scheduling involving the inactive professional, and reactivation restores the associations. Inactivation does not alter existing attendance participation or prevent otherwise-allowed evolution updates. See ADR-016.

Detailed flows and acceptance criteria are in [05 — Use Cases](../05-use-cases.md).

## Consequences

- **Benefits:** Defines association cardinality, permits adding a professional through the patient record, and allows a patient to exist without a professional. Scheduling-flow association creation is defined by ADR-017.
- **Costs or risks:** Professional deletion semantics remain open.
- **Follow-up actions:** Implement association removal while preserving attendance/evolution history; resolve professional deletion semantics separately. Scheduling-flow association creation is defined by ADR-017.
