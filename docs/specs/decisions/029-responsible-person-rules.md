# ADR-029: Responsible-Person Rules

**Status:** Accepted
**Date:** 2026-11-11

## Context

Responsible-person cardinality, same-flow creation, kinship values, and duplicate handling were open.

## Options considered

1. Permit multiple responsible persons per patient and reuse a responsible-person record across patients.
2. Permit multiple responsible persons per patient, with each record belonging to one patient.
3. Limit each patient to one responsible person.

## Decision

A patient may have multiple responsible-person records, and each record belongs to exactly one patient. A responsible person may be created while registering or editing the patient. Duplicate names for the same patient are rejected. Degree of kinship is selected from a closed list, whose values and name-normalization rule remain open.

## Consequences

- **Benefits:** Keeps each responsible person's contacts scoped to one patient and prevents duplicate names within that patient's records.
- **Costs or risks:** Reusing the same person's record across patients is not supported; allowed kinship values still need confirmation.
- **Follow-up actions:** Define the closed kinship list and name-normalization rule before implementing responsible-person registration.
