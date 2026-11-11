# ADR-033: Anamnesis Revision and Clinical-Data Gate

**Status:** Accepted
**Date:** 2026-11-11

## Context

Anamnesis section prompts, revision metadata, retention, and clinical-data handling requirements were open. Attendance/evolution privacy and legal/data-handling requirements were also unresolved.

## Options considered

1. Use free-text fields for the confirmed anamnesis sections and preserve version metadata.
2. Define detailed structured prompts before implementing anamnesis.

## Decision

Use free-text fields for the already confirmed anamnesis sections. Record the date, author, and timestamp for each version. Do not automatically delete anamnesis records while the retention policy is unresolved. Keep anamnesis, attendance, and evolution implementation blocked until privacy/legal review defines consent/legal basis, data handling, and retention requirements.

## Consequences

- **Benefits:** Keeps the confirmed anamnesis scope simple and preserves revision accountability.
- **Costs or risks:** Clinical-feature implementation remains blocked; final retention behavior is not yet determined.
- **Follow-up actions:** Complete privacy/legal review and define clinical-data handling and retention before unblocking the affected roadmap items.
