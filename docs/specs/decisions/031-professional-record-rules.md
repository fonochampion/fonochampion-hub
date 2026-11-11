# ADR-031: Professional Record Rules

**Status:** Accepted
**Date:** 2026-11-11

## Context

Professional basic fields, CPF requiredness/validation, certification multiplicity, and contact rules were open.

## Options considered

1. Require full name and CPF, allow optional multiple certifications, and require at least one contact.
2. Make CPF, certifications, and contacts optional.

## Decision

Require full name and a valid, unique CPF. Certifications are optional and may have multiple entries; each contains a description, alphanumeric registration, and completion date. At least one telephone or email contact is required, multiple values of either type are allowed, and contact values must pass format validation. Which fields are editable and how certifications and contacts are individually maintained remain open.

## Consequences

- **Benefits:** Establishes the minimum professional identity and contact information while allowing multiple certifications and contacts.
- **Costs or risks:** CPF and at least one contact are required for every professional.
- **Follow-up actions:** Define editable-field and individual certification/contact maintenance rules before implementing professional maintenance.
