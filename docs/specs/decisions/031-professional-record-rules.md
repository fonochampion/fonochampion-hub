# ADR-031: Professional Record Rules

**Status:** Accepted
**Date:** 2026-11-11

## Context

Professional basic fields, CPF requiredness/validation, certification multiplicity, and contact rules were open.

## Options considered

1. Require full name and CPF, allow optional multiple certifications, and require at least one contact.
2. Make CPF, certifications, and contacts optional.
3. Accept CPF input with or without the standard punctuation and normalize it, or accept digits only.
4. Reuse the confirmed Brazilian telephone format or accept international telephone formats.
5. Restrict a certification completion date to today or earlier, or allow future-dated plans.

## Decision

Require full name and a unique CPF that passes check-digit validation. Accept either 11 digits or the standard `000.000.000-00` format, and store and compare only the 11 digits. Certifications are optional and may have multiple entries; each contains a description, alphanumeric registration, and a completion date that cannot be in the future. Require at least one telephone or email contact and allow multiple values of either type. Telephone contacts use the Brazilian DDD and subscriber number format (10 or 11 digits), with optional `+55` and spaces, parentheses, or hyphens as separators. Email contacts must pass syntax validation. Contact values are trimmed at their boundaries, are not otherwise normalized, and are not verified for ownership. These rules apply to initial registration; editable fields and individual certification/contact maintenance remain open for UC-005.

## Consequences

- **Benefits:** Establishes consistent CPF uniqueness and contact validation for professional registration while allowing multiple certifications and contacts.
- **Costs or risks:** CPF and at least one contact are required for every professional; contact ownership is not verified.
- **Follow-up actions:** Define editable-field and individual certification/contact maintenance rules before implementing UC-005.
