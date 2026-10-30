# ADR-010: Patient Attendance and Evolution Records

**Status:** Superseded by ADR-013 on 2026-10-30
**Date:** 2026-10-29

## Context

The initial product stage must support maintaining records of completed patient attendances. At the end of an attendance, participating professionals may record an evolution describing the therapeutic plan carried out and observations. Scheduling is planned for a later stage.

## Options considered

1. Defer attendance recording until scheduling is available.
2. Record completed attendances now, allowing date/time to be absent until future scheduling supplies them.

## Decision

- The current scope records completed attendances only; it does not include appointment scheduling.
- An attendance concerns exactly one patient and one or more active professionals already associated with that patient.
- An active professional may be added to the patient through the patient record after initial registration. At least one such association must exist before recording an attendance; the attendance flow cannot create the association itself.
- Each participating professional has a separate evolution entry. An evolution is optional and summarizes the therapeutic plan carried out and the professional's observations during the attendance.
- The system indicates when an evolution has not yet been recorded. A participating professional may record or update evolution entries; updates preserve previous versions.
- Until scheduling is available, an attendance may be recorded without date or time. When scheduling is introduced, every attendance must be linked to an appointment and obtain its date and time from that appointment.
- Removing a patient-professional association and its effect on historical attendances, attendance/evolution deletion or cancellation, evolution revision metadata, and access, privacy, retention, and audit rules remain open.

Detailed flows and acceptance criteria are in [05 — Use Cases](../05-use-cases.md).

## Consequences

- **Benefits:** Clinical attendance and evolution records can be maintained before the scheduling capability exists, while preserving the future appointment relationship.
- **Costs or risks:** These records contain clinical information. Access, privacy, retention, audit, revision, and deletion/cancellation rules must be resolved before implementing the dependent functionality.
- **Follow-up actions:** Resolve the open controls and lifecycle questions before implementation; specify scheduling separately before introducing mandatory appointment links.
