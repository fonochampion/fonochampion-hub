# ADR-027: Patient Date of Birth and Age Display

**Status:** Accepted
**Date:** 2026-11-10

## Context

The patient record requires an age value, but its representation and validation were open. The product must also support patients younger than one year.

## Options considered

1. Store an age value directly.
2. Store the patient's date of birth and derive age for display.

## Decision

- Require the patient's date of birth.
- Reject dates of birth in the future; do not set a maximum age.
- Derive age from the date of birth. Display age in months for patients younger than one year and in completed years thereafter.

## Consequences

- **Benefits:** Age remains current without periodic edits, and infants can be represented in months.
- **Costs or risks:** The system stores a date of birth rather than only an age value.
- **Follow-up actions:** Implement the confirmed rules in patient registration under IMP-005.
