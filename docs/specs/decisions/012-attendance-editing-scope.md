# ADR-012: Attendance Editing Scope

**Status:** Accepted
**Date:** 2026-10-30

## Context

The initial attendance use case defines adding participating professionals and recording or updating their evolution entries. It needed to clarify whether other attendance data could also be edited after recording.

## Options considered

1. Allow editing additional attendance data.
2. Limit maintenance to the explicitly defined operations.

## Decision

The defined attendance maintenance operations are adding participating professionals, removing a participant under the constraints in [ADR-013](./013-initial-attendance-scheduling-and-lifecycle.md), and recording or updating evolution entries. No other attendance data may be edited.

## Consequences

- **Benefits:** The initial scope has a clear and limited set of attendance maintenance operations.
- **Costs or risks:** Changes to other attendance data are not supported in this scope.
- **Follow-up actions:** Attendance and evolution records are retained permanently and cannot be deleted. Resolve the remaining access and privacy questions.
