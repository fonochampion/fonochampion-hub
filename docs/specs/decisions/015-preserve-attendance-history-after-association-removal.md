# ADR-015: Preserve Attendance History After Association Removal

**Status:** Accepted
**Date:** 2026-10-30

## Context

Professional associations with patients may be removed after registration. Existing attendance and evolution records need a defined outcome when their patient-professional association is removed.

## Options considered

1. Prevent association removal when attendance or evolution records exist.
2. Allow association removal and retain existing attendance and evolution records unchanged.

## Decision

An association may be removed without deleting or altering existing attendance or evolution records. Any already scheduled attendance and its participants remain unchanged.

This decision resolves the association-removal history question in ADR-008. Professional inactivation is addressed separately in ADR-016.

## Consequences

- **Benefits:** Association removal does not erase the permanent attendance/evolution history.
- **Costs or risks:** No additional association-removal effects were specified beyond preserving existing attendance records and scheduled participation.
- **Follow-up actions:** Implement association removal according to this decision.
