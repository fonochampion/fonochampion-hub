# ADR-017: Scheduling Creates a Professional Association

**Status:** Accepted
**Date:** 2026-10-30

## Context

The attendance lifecycle requires one or more active professionals associated with the patient. The prior rule required that association before scheduling, which prevented scheduling for a patient with no active associated professional. Rescheduling also needed a rule for an inactive participant.

## Options considered

1. Require patient-professional associations to be created through the patient record before scheduling.
2. Allow the scheduling flow to establish associations when no active associated professional is available.

## Decision

- Any system user with scheduling permission may schedule or reschedule; which roles receive this permission remains open.
- If active professionals are already associated with the patient, the scheduling flow offers those professionals for selection.
- If the patient has no active associated professional, the flow offers all active, registered professionals. Selecting one or more creates the patient-professional associations and completes scheduling.
- When rescheduling, retain existing active participants and remove inactive participants from the replacement. If no active participant remains, offer active professionals already associated with the patient; if none are actively associated, offer all active professionals and create associations for the selection.

This decision supersedes ADR-008's former restriction that an attendance flow cannot create a patient-professional association.

## Consequences

- **Benefits:** A patient with no active professional association can still be scheduled without leaving the scheduling flow.
- **Costs or risks:** Which user roles may schedule remains open and must be specified before authorization is implemented.
- **Follow-up actions:** Define the roles authorized to schedule and apply the scheduling flow rules in UC-008.
