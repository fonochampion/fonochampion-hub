# ADR-034: Attendance Cancellation Reasons

**Status:** Accepted
**Date:** 2026-11-11

## Context

Administrators maintain configurable attendance cancellation reasons, but the complete set of values was open. `Remarcação` is already a fixed system reason.

## Options considered

1. Keep only the existing patient nonattendance reason.
2. Add professional unavailability and patient/responsible-person request as configurable reasons.

## Decision

The configurable reasons are `Não comparecimento`, `Indisponibilidade do profissional`, and `Solicitação do paciente/responsável`. The optional free-text detail remains available. `Remarcação` remains a fixed system reason assigned automatically during rescheduling.

## Consequences

- **Benefits:** Defines a small set of operational cancellation reasons and retains context through optional detail.
- **Costs or risks:** New situations must use the optional detail until the reason set is explicitly changed.
- **Follow-up actions:** None.
