# ADR-035: Professional Record Maintenance

**Status:** Accepted
**Date:** 2026-11-11

## Context

Professional modification was confirmed, but editable fields, certification/contact operations, minimum contact requirements after editing, and retention of prior values were open.

## Options considered

1. Make only the full name editable or allow both the full name and CPF to be edited.
2. Replace certification/contact lists on each save or add, edit, and remove their entries individually.
3. Keep at least one valid contact after registration or allow a professional to have none.
4. Retain prior certification/contact versions or keep only current values.

## Decision

Administrators may edit a professional's full name and CPF. The trimmed name must remain nonempty, and an edited CPF follows the input and normalization rules in [ADR-031](./031-professional-record-rules.md), passes check-digit validation, and remains unique across other professionals. Certifications are optional and are added, edited, or removed individually; added or edited certifications follow the existing description, alphanumeric registration, and non-future completion-date rules. Telephone and email contacts are added, edited, or removed individually, and at least one valid contact must remain. Added or edited contacts follow the existing type, format, and trimming rules. Certification and contact changes retain only current values; prior versions are not preserved.

## Consequences

- **Benefits:** Allows correcting professional identity data and maintaining certifications and contact details without replacing unrelated entries, while preserving required contactability.
- **Costs or risks:** Removing the last valid contact is not allowed; prior certification and contact values cannot be recovered from the application.
- **Follow-up actions:** Implement the confirmed modification workflow in IMP-009.
