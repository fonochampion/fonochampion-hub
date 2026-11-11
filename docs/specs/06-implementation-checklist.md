# Implementation Roadmap

**Status:** Working roadmap
**Last reviewed:** 2026-11-11

This is the master, hierarchical list of approved implementation work. The specifications in this directory remain the source of truth for scope and behavior; this roadmap tracks implementation status and sequence. Its order reflects dependencies, not priority or a commitment to implement every item in one delivery.

## Status and agent workflow

Use exactly these statuses for implementation items:

- `PENDING` — approved work not started.
- `BLOCKED` — cannot be implemented safely until a listed decision or dependency is resolved.
- `IN PROGRESS` — implementation has started but is not complete and validated.
- `DONE` — implementation is complete and its relevant tests/checks have passed.

When asked what remains to be implemented, report `PENDING`, `BLOCKED`, and `IN PROGRESS` items in roadmap order, grouped by phase and feature. Keep blockers visible and explain what decision or dependency is needed. Exclude `DONE` items from that list unless completion history is requested.

Before starting an item, set it to `IN PROGRESS`. Set it to `DONE` only after implementation and relevant validation are complete; code written without passing validation is not done. If validation fails, keep it `IN PROGRESS` and report the failure. Do not mark a whole feature done when only part of its scope is complete.

When a new feature or approved scope change is specified, add its implementation items in the appropriate hierarchy before implementation, with stable IDs, dependencies, and acceptance evidence. Do not add unapproved features or turn open questions into requirements. Preserve completed items and their IDs; do not silently reopen or remove them.

## Readiness and priority

Readiness is separate from status: keep the existing status values and mark an item ready only when its scope is sufficiently specified, all listed dependencies are `DONE`, and all relevant decision gates are resolved. Prioritize ready items with no dependencies before ready items whose dependencies are complete. Keep blocked items visible with the exact dependency or open decision that prevents implementation.

IMP-001, IMP-002, IMP-003, IMP-004, IMP-005, IMP-006, IMP-007, IMP-008, IMP-015, IMP-018, IMP-020, IMP-021, and IMP-022 are complete; no pending Phase 1 item is currently ready. Reassess other pending work against its listed dependencies and decision gates. Production hosting and deployment remain out of scope. Reassess readiness whenever specifications, decisions, dependencies, or repository instructions change.

## Phase 0 — Resolve specification gates

These are product decisions, not implementation tasks. Resolve each in the specifications before implementing work that depends on it. Relevant items below remain `BLOCKED` until their gates are closed.

| Gate | Open decision | Source |
|---|---|---|
| DG-001 | RESOLVED — require date of birth, reject future dates, set no maximum age, and derive age for display in months below one year and completed years thereafter | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [ADR-027](./decisions/027-patient-date-of-birth-and-age-display.md) |
| DG-002 | RESOLVED — use a reusable, read-only, versioned local CID-10 catalog; no runtime CID API | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [ADR-026](./decisions/026-local-cid10-reference-catalog.md) |
| DG-003 | RESOLVED — allow multiple responsible-person records per patient, each belonging to one patient; create within patient registration/editing; reject duplicate names using trimmed, whitespace-collapsed, case-insensitive comparison while preserving accents and punctuation; use the confirmed closed kinship list without a free-text `Other` detail | [UC-002](./05-use-cases.md#uc-002--register-and-associate-a-responsible-person), [ADR-029](./decisions/029-responsible-person-rules.md) |
| DG-004 | RESOLVED — require at least one telephone or email, allow multiple of either, use no preferred contact, and validate contact formats | [UC-003](./05-use-cases.md#uc-003--register-contact-methods-for-a-responsible-person), [ADR-030](./decisions/030-responsible-person-contact-rules.md) |
| DG-005 | PARTIALLY RESOLVED — initial registration requires full name and a valid unique CPF, optional certifications, and at least one valid telephone or email; CPF, telephone, email, and completion-date rules are defined. Editable fields and individual certification/contact maintenance remain open for UC-005 | [UC-004–UC-005](./05-use-cases.md#use-cases), [ADR-031](./decisions/031-professional-record-rules.md) |
| DG-006 | PARTIALLY RESOLVED — first-delivery authentication, user fields, fixed roles, bootstrap Administrator, and temporary-password flow are defined; account deletion remains unspecified and password reset/recovery is deferred | [UC-009](./05-use-cases.md#uc-009--manage-system-user-accounts-and-roles), [ADR-019](./decisions/019-first-delivery-authentication-and-user-management.md) |
| DG-007 | RESOLVED — logical, terminal deletion is distinct from reversible inactivation; preserve history and allow deletion only when no patient is associated | [UC-007](./05-use-cases.md#uc-007--delete-a-professional), [ADR-032](./decisions/032-professional-deletion-lifecycle.md) |
| DG-008 | PARTIALLY RESOLVED — anamnesis uses free-text confirmed sections and version date/author/timestamp; no automatic deletion pending retention policy. Clinical implementation remains blocked pending privacy/legal review | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [Data and Integrations](./04-data-and-integrations.md#privacy-and-security), [ADR-033](./decisions/033-anamnesis-revision-and-clinical-data-gate.md) |
| DG-009 | BLOCKED — implementation of anamnesis, attendance, and evolution must wait for privacy/legal review to define consent/legal basis, data handling, and retention; role access is already defined | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [Data and Integrations](./04-data-and-integrations.md#privacy-and-security), [ADR-033](./decisions/033-anamnesis-revision-and-clinical-data-gate.md) |
| DG-010 | RESOLVED — association removal preserves existing attendance/evolution records and leaves already scheduled attendance participants unchanged | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [ADR-015](./decisions/015-preserve-attendance-history-after-association-removal.md) |
| DG-011 | RESOLVED — fixed duration options are 10, 20, 30, 40, 50, or 60 minutes per attendance; rescheduling requires a new selection | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [ADR-013](./decisions/013-initial-attendance-scheduling-and-lifecycle.md) |
| DG-012 | RESOLVED — configurable reasons are `Não comparecimento`, `Indisponibilidade do profissional`, and `Solicitação do paciente/responsável`; fixed `Remarcação` remains automatic | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [ADR-034](./decisions/034-attendance-cancellation-reasons.md) |
| DG-013 | RESOLVED — rescheduling removes inactive participants; if none remain, select active associated professionals or create associations from the active registry | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [ADR-017](./decisions/017-scheduling-creates-professional-association.md) |
| DG-014 | RESOLVED — display name is required, trimmed, and nonempty after trimming; email is optional, trimmed, and format-validated when provided, with no uniqueness or ownership verification | [UC-009](./05-use-cases.md#uc-009--manage-system-user-accounts-and-roles), [ADR-024](./decisions/024-system-user-account-field-rules.md) |
| DG-015 | Password reset/recovery behavior and security requirements for its later delivery | [UC-009](./05-use-cases.md#uc-009--manage-system-user-accounts-and-roles) |
| DG-016 | RESOLVED — matching existing Administrator accounts are left unchanged; account conflicts and a configured username that differs from an existing Administrator fail startup without creating or changing an account | [UC-009](./05-use-cases.md#uc-009--manage-system-user-accounts-and-roles), [ADR-023](./decisions/023-initial-administrator-bootstrap-lifecycle.md) |
| DG-017 | RESOLVED — requester directs use of supplied files identified as SUS data without further source/version/terms research; use is internal only, and files/catalog data must not be committed, packaged, or redistributed | [Data and Integrations](./04-data-and-integrations.md#data), [ADR-026](./decisions/026-local-cid10-reference-catalog.md) |
| DG-018 | RESOLVED — reject patient creation when normalized full name and date of birth match; trim leading/trailing whitespace, collapse internal whitespace, compare case-insensitively, and preserve accents and punctuation | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [ADR-028](./decisions/028-patient-duplicate-detection.md) |

## Phase 1 — First delivery: foundation and access management

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-001 | Create the .NET solution and confirmed `src/Web`, `src/Application`, `src/Domain`, and `src/Infrastructure` project structure. | — | DONE |
| IMP-002 | Configure ASP.NET Core MVC, Entity Framework Core, Npgsql, and PostgreSQL persistence in the confirmed modular-monolith structure. | IMP-001 | DONE |
| IMP-003 | Add the root `Dockerfile` and `docker-compose.yml` for local ASP.NET Core and PostgreSQL execution using the confirmed ports, local environment-variable names, persistent volume, database healthcheck, and startup migrations. | IMP-001, IMP-002 | DONE |
| IMP-004 | Build the shared Bootstrap 5 responsive shell: header, collapsible navigation, content area, footer, and confirmed color system. | IMP-001 | DONE |
| IMP-021 | Implement local username/password authentication, authenticated sessions, and the required first-login password change flow using the confirmed username, password, session, and failed-login policies. | IMP-002 | DONE |
| IMP-022 | Provision the initial Administrator from configuration supplied outside the source code and require a password change at first login. | IMP-021 | DONE |
| IMP-020 | Manage system-user accounts: create/edit accounts with username, display name, and email attribute; assign one of the three fixed roles; activate/inactivate accounts; generate a temporary password shown once; and require its change at first login. Password reset/recovery and role-definition management are excluded from this delivery. | IMP-022 | DONE |

## Phase 2 — Business backlog: shared reference data, patient, and responsible-person features

| ID | Feature | Depends on | Status |
|---|---|---|---|
| IMP-026 | Import the reusable, read-only Brazilian Portuguese CID-10 catalog from a local directory supplied in each environment through an explicit command; preserve its chapter/group/category/subcategory hierarchy and lookup by code or description; identify each file set by content fingerprint when no upstream version is available; exclude CID-O and do not commit, package, or redistribute the files or catalog data. | IMP-002 | DONE |
| IMP-005 | Implement patient record creation with full name, required date of birth, and CID selected from the local catalog. | IMP-002, IMP-026 | DONE |
| IMP-016 | Record optional dated anamneses for patients, including the confirmed sections, separate entries, and preserved revision history. | IMP-005; DG-008 | BLOCKED |
| IMP-006 | Register a responsible person with full name and degree of kinship and associate them with a patient. | IMP-005; DG-003 | DONE |
| IMP-007 | Register telephone and email contact details for a responsible person. | IMP-006; DG-004 | DONE |

## Phase 3 — Business backlog: professional, scheduling, and attendance features

| ID | Feature | Depends on | Status |
|---|---|---|---|
| IMP-008 | Register a fonoaudiólogo with confirmed basic data, unique CPF, certifications, and telephone/email contacts. | IMP-002; DG-005 (registration scope) | DONE |
| IMP-015 | Allow association of one or more existing, active professionals during initial patient registration, later through the patient record, and through scheduling when no active association exists; support each professional being associated with multiple patients. | IMP-005, IMP-008 | DONE |
| IMP-017 | Schedule and maintain patient attendances with required scheduled date/time and a fixed duration option per attendance, patient/professional overlap prevention, scheduling-flow association creation when needed, status transitions, actual realization timestamps, late indication, rescheduling with inactive-participant removal and a new duration selection, cancellation, and one current evolution entry per professional. | IMP-015, IMP-018; DG-009, DG-012 | BLOCKED |
| IMP-018 | Provide configurable cancellation-reason maintenance with create, list, edit, and inactivate operations; exclude the fixed `Remarcação` system reason, which is assigned automatically during rescheduling. | IMP-002; DG-012 | DONE |
| IMP-019 | Remove patient-professional associations without deleting or altering existing attendance/evolution records or changing already scheduled attendance participants. | IMP-015, IMP-017 | BLOCKED |
| IMP-009 | Modify a professional, including the confirmed CPF uniqueness and certification/contact maintenance rules. | IMP-008; DG-005 | BLOCKED |
| IMP-010 | Inactivate/reactivate a professional, preserve patient associations during inactivity, apply the confirmed scheduling restriction, and preserve existing attendance participation/evolution rights. | IMP-008 | BLOCKED |
| IMP-011 | Delete a professional only when no patient is associated, using the confirmed deletion semantics. | IMP-008; DG-007 | BLOCKED |

## Phase 4 — Cross-cutting completion for the business backlog

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-012 | Apply confirmed authentication and authorization rules to implemented business operations. | IMP-005–IMP-011, IMP-015–IMP-022; DG-008, DG-009 | BLOCKED |
| IMP-013 | Verify implemented screens on desktop, tablet, and smartphone, including the shared layout and confirmed color system. | IMP-004–IMP-011, IMP-015–IMP-022 | BLOCKED |
| IMP-014 | Run the relevant automated tests and project checks for the approved implementation scope; record the results with the delivery. | IMP-001–IMP-013, IMP-015–IMP-022, IMP-024, IMP-026 | BLOCKED |

## Phase 5 — Deferred account-management backlog

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-024 | Add the Administrator-initiated password reset/recovery capability for system-user accounts. | IMP-020; DG-015 | BLOCKED |

## Phase 6 — Agent guidance and skills

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-025 | Clarify Copilot instruction entry points and skill responsibilities; remove duplicated or stale instructions and verify discovery using Copilot CLI. | — | DONE |

## Deferred or excluded from the current roadmap

Do not implement these unless they are specified and approved as a future scope change:

- Public API retrieval of patient CID.
- Public API retrieval of professional council-registration information.
- Billing and other features not confirmed in the current scope.
