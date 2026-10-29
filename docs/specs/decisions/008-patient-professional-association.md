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
- At least one active professional must already be associated with a patient before an attendance can be recorded. An attendance cannot itself create the patient-professional association.
- A professional associated with one or more patients cannot be deleted.
- Whether an association can be removed from a patient, and the effects of removal on existing attendance records, remain open.
- The effects of professional inactivation on existing patient associations remain open.

Detailed flows and acceptance criteria are in [05 — Use Cases](../05-use-cases.md).

## Consequences

- **Benefits:** Defines association cardinality, permits adding a professional through the patient record, and establishes the prerequisite for recording attendances while allowing a patient to exist without a professional.
- **Costs or risks:** Removal of associations and the effects of professional inactivation or association removal on attendance records still require decisions.
- **Follow-up actions:** Resolve the open lifecycle questions before implementing association removal or related attendance-history behavior.
