# ADR-028: Patient Duplicate Detection

**Status:** Accepted
**Date:** 2026-11-11

## Context

Patient registration has no unique identifier beyond the confirmed full name and date of birth. Duplicate handling was open.

## Options considered

1. Allow exact matches without a warning.
2. Warn on a match but allow registration.
3. Reject a match.

## Decision

Reject patient creation when another patient has the same date of birth and a matching full name. Normalize the name by trimming leading and trailing whitespace, collapsing internal whitespace, and comparing without case sensitivity; preserve accents and punctuation.

## Consequences

- **Benefits:** Prevents duplicate records for the confirmed match while keeping the comparison rule explicit.
- **Costs or risks:** Two different people with the same normalized name and date of birth cannot be separately registered.
- **Follow-up actions:** Define patient update and archival behavior separately.
