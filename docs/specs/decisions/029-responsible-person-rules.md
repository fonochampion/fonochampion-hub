# ADR-029: Responsible-Person Rules

**Status:** Accepted
**Date:** 2026-11-11

## Context

Responsible-person cardinality, same-flow creation, kinship values, and duplicate handling were open.

## Options considered

1. Permit multiple responsible persons per patient and reuse a responsible-person record across patients.
2. Permit multiple responsible persons per patient, with each record belonging to one patient.
3. Limit each patient to one responsible person.
4. Use an essential kinship list or an expanded list; treat `Outro` as a fixed value or allow a free-text detail.
5. Normalize duplicate names using the patient rule or trim outer whitespace and ignore case without collapsing internal whitespace.

## Decision

A patient may have multiple responsible-person records, and each record belongs to exactly one patient. A responsible person may be created while registering or editing the patient. Duplicate names for the same patient are rejected after trimming leading and trailing whitespace, collapsing internal whitespace, and comparing without case sensitivity while preserving accents and punctuation. Degree of kinship is selected from the closed list `Mãe`, `Pai`, `Avó/Avô`, `Irmã/Irmão`, `Tia/Tio`, `Padrasto/Madrasta`, `Cônjuge/Companheiro(a)`, `Tutor/Guardião legal`, and `Outro`. `Outro` is a fixed value with no free-text detail.

## Consequences

- **Benefits:** Keeps each responsible person's contacts scoped to one patient, provides an explicit kinship list, and applies consistent duplicate-name matching across patient and responsible-person records.
- **Costs or risks:** Reusing the same person's record across patients is not supported; the fixed `Outro` value cannot capture a more specific relationship.
- **Follow-up actions:** None for the kinship list or duplicate-name rule.
