# ADR-034: Attendance Cancellation Reasons

**Status:** Accepted
**Date:** 2026-11-11

## Context

Administrators maintain configurable attendance cancellation reasons, but the complete set of values, initial records, and name matching rule were open. `Remarcação` is already a fixed system reason.

## Options considered

1. Keep only the existing patient nonattendance reason.
2. Add professional unavailability and patient/responsible-person request as configurable reasons.
3. Install the three specified reasons as active initial records or leave the list empty for the Administrator to populate.
4. Require exact name uniqueness or compare trimmed names without case sensitivity.

## Decision

The configurable reasons are `Não comparecimento`, `Indisponibilidade do profissional`, and `Solicitação do paciente/responsável`; they are created as active initial records when the feature is first installed. Reason names are required and unique across active and inactive records after trimming leading and trailing whitespace and comparing without case sensitivity while preserving accents and punctuation. The optional free-text detail remains available. `Remarcação` remains a fixed system reason assigned automatically during rescheduling and is excluded from configurable maintenance.

## Consequences

- **Benefits:** Provides the confirmed reasons on first use and prevents duplicate labels across active and inactive records while retaining context through optional detail.
- **Costs or risks:** New situations must use the optional detail until the reason set is explicitly changed; changes in case or outer spacing do not create distinct reason names.
- **Follow-up actions:** None.
