# ADR-013: Initial Attendance Scheduling and Lifecycle

**Status:** Accepted
**Date:** 2026-10-30

## Context

The initial attendance scope was limited to completed records, with scheduling deferred. The requester expanded the current scope to include scheduling and the lifecycle from scheduling through realization and evolution.

## Options considered

1. Keep scheduling deferred and preserve completed-attendance-only records.
2. Include scheduling and the attendance lifecycle in the current scope.

## Decision

- The patient nonattendance reason may be selected at any time while an attendance is `Agendado`, including before its scheduled date/time.
- The current scope includes scheduling and attendance realization.
- Administrators and Attendants may schedule or reschedule. Their other permissions are specified in ADR-018.
- Every attendance has a required scheduled date/time and one or more professionals who are active and associated with its patient when scheduling completes. If no active professional is associated, the scheduling flow offers all active professionals and creates associations for the selection, as specified by ADR-017.
- Each attendance has a duration selected from the fixed options of 10, 20, 30, 40, 50, or 60 minutes. The scheduled date/time and duration are used to prevent overlapping attendances for patients and professionals. Rescheduling creates a replacement with a new duration selection.
- Attendants and Administrators may add additional active, registered professionals already associated with the patient while the attendance is in `Agendado`, `Em andamento`, `Aguardando Evolução`, or `Finalizado`; they may not be added when it is `Cancelado`.
- A participating professional may be removed only while the attendance is `Agendado`, only if another participant remains, and only if the removed professional has no evolution recorded.
- Scheduled and realization periods are distinct. Actual start and end date/time are recorded automatically when a participating professional starts and ends the attendance. Any participating professional may start or end the attendance before or after the scheduled date/time.
- The statuses are `Agendado`, `Em andamento`, `Finalizado`, `Aguardando Evolução`, and `Cancelado`. Ending an attendance sets it to `Finalizado` if at least one evolution exists, or `Aguardando Evolução` otherwise. Adding an evolution afterward automatically changes the status to `Finalizado`; participants still lacking their own evolution remain individually indicated as pending.
- The system preserves each status change with its author and date/time. For an automatic transition, the author is the professional or user whose action triggered it.
- Attendance records and evolution entries are retained permanently and cannot be deleted, including cancelled attendances.
- Only an attendance in `Agendado` may be cancelled. Any participating professional or an Administrator may cancel it. Cancellation requires a reason selected from a reason domain; free-text detail is optional. Administrators maintain configurable reasons, including patient nonattendance, with create, list, edit, and inactivate operations; each reason has a required name, optional description, and active/inactive indicator, with names unique across active and inactive records. Configurable reasons cannot be deleted. An inactivated reason is unavailable for new cancellations but remains visible on historical attendances; editing a reason does not change the original name and description shown in historical attendances. Remaining configurable values are open. `Remarcação` is a fixed system reason not maintained through that feature.
- Rescheduling automatically cancels the current scheduled attendance and creates a replacement for the same patient, linked to the prior record, with a new date/time and duration. Existing active participants remain; inactive participants are removed. If none remain, the system offers active associated professionals or, if none are associated, all active professionals and creates associations for the selection. The system assigns the fixed `Remarcação` reason. See ADR-017.
- When scheduled date/time passes before start, the attendance remains `Agendado`; the system indicates lateness on screen by comparing scheduled and current date/time. A professional may still start the attendance.
- Attendance records cannot be deleted, including records with status `Cancelado`.
- Evolution entries cannot be deleted. Updates replace the current content; prior versions are not retained, as specified by ADR-014.
- Overlapping attendances for a professional are prevented when the existing attendance is `Agendado`; `Em andamento`, `Cancelado`, and `Finalizado` attendances do not count as scheduling conflicts. Overlapping attendances for the same patient are also prevented.
- Each participating professional may record or update only their own optional evolution entry while the attendance is `Em andamento` or `Aguardando Evolução`. Evolution cannot be recorded or updated in other statuses. Updates replace the current entry; prior versions are not retained, as specified by ADR-014.
- Administrators and Therapists may access clinical attendance and evolution data for all patients. Administrators cannot alter another professional's evolution and may start or end an attendance only when participating; Attendants may access operational scheduling data but not anamnesis or evolution content. See ADR-018.
- An evolution entry has separate fields for the therapeutic plan carried out and the professional's observations. The plan is required; observations are optional.

Detailed flows and remaining open questions are in [05 — Use Cases](../05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance).

## Consequences

- **Benefits:** The initial product supports the full scheduled-to-realized attendance flow while retaining a distinct history for rescheduling.
- **Costs or risks:** Authentication, remaining cancellation-reason values, and clinical-data privacy/handling requirements remain open.
- **Follow-up actions:** Apply the role permissions in ADR-018 and resolve the remaining UC-008 questions before implementation.
