# Implementation Roadmap

**Status:** Working roadmap
**Last reviewed:** 2026-10-31

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

IMP-001, IMP-002, and IMP-004 are complete. IMP-003 is the next ready item, with local-only settings defined in [02 — Architecture](./02-architecture.md#local-development-environment) and [ADR-021](./decisions/021-docker-local-development-configuration.md). Production hosting and deployment remain out of scope.

| Priority | ID | Implementation item | Depends on | Status |
|---|---|---|---|---|
| 1 | IMP-003 | Add the root `Dockerfile` and `docker-compose.yml` for local ASP.NET Core and PostgreSQL execution using the confirmed local configuration. | IMP-001, IMP-002 | PENDING |

This is a readiness classification, not authorization to start implementation. Implement only roadmap items explicitly selected by the requester. Reassess readiness whenever specifications, decisions, dependencies, or repository instructions change.

## Phase 0 — Resolve specification gates

These are product decisions, not implementation tasks. Resolve each in the specifications before implementing work that depends on it. Relevant items below remain `BLOCKED` until their gates are closed.

| Gate | Open decision | Source |
|---|---|---|
| DG-001 | Age representation and validation | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis) |
| DG-002 | How CID is managed before the deferred public API integration | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis) |
| DG-003 | Kinship values, patient/responsible-person cardinality, and duplicate handling | [UC-002](./05-use-cases.md#uc-002--register-and-associate-a-responsible-person) |
| DG-004 | Contact requiredness, multiplicity, and validation | [UC-003](./05-use-cases.md#uc-003--register-contact-methods-for-a-responsible-person) |
| DG-005 | Professional basic-data fields, CPF requiredness/validation, certification and contact rules, and editable fields | [UC-004–UC-005](./05-use-cases.md#use-cases) |
| DG-006 | PARTIALLY RESOLVED — first-delivery authentication, user fields, fixed roles, bootstrap Administrator, and temporary-password flow are defined; account deletion remains unspecified and password reset/recovery is deferred | [UC-009](./05-use-cases.md#uc-009--manage-system-user-accounts-and-roles), [ADR-019](./decisions/019-first-delivery-authentication-and-user-management.md) |
| DG-007 | Professional deletion semantics | [UC-007](./05-use-cases.md#uc-007--delete-a-professional) |
| DG-008 | Anamnesis privacy/retention, detailed section prompts, and revision metadata; role access is defined in ADR-018 | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [Data and Integrations](./04-data-and-integrations.md#privacy-and-security) |
| DG-009 | Attendance/evolution privacy and legal/data-handling requirements; role access is defined in ADR-018 | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [Data and Integrations](./04-data-and-integrations.md#privacy-and-security) |
| DG-010 | RESOLVED — association removal preserves existing attendance/evolution records and leaves already scheduled attendance participants unchanged | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [ADR-015](./decisions/015-preserve-attendance-history-after-association-removal.md) |
| DG-011 | RESOLVED — fixed duration options are 10, 20, 30, 40, 50, or 60 minutes per attendance; rescheduling requires a new selection | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [ADR-013](./decisions/013-initial-attendance-scheduling-and-lifecycle.md) |
| DG-012 | PARTIALLY RESOLVED — Administrators maintain configurable cancellation reasons; remaining reason values are open (excluding fixed `Remarcação`) | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [ADR-018](./decisions/018-initial-user-roles-and-permissions.md) |
| DG-013 | RESOLVED — rescheduling removes inactive participants; if none remain, select active associated professionals or create associations from the active registry | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [ADR-017](./decisions/017-scheduling-creates-professional-association.md) |
| DG-014 | Account-field requiredness and validation, username normalization, password policy, and detailed bootstrap lifecycle | [UC-009](./05-use-cases.md#uc-009--manage-system-user-accounts-and-roles), [ADR-019](./decisions/019-first-delivery-authentication-and-user-management.md) |
| DG-015 | Password reset/recovery behavior and security requirements for its later delivery | [UC-009](./05-use-cases.md#uc-009--manage-system-user-accounts-and-roles) |

## Phase 1 — First delivery: foundation and access management

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-001 | Create the .NET solution and confirmed `src/Web`, `src/Application`, `src/Domain`, and `src/Infrastructure` project structure. | — | DONE |
| IMP-002 | Configure ASP.NET Core MVC, Entity Framework Core, Npgsql, and PostgreSQL persistence in the confirmed modular-monolith structure. | IMP-001 | DONE |
| IMP-003 | Add the root `Dockerfile` and `docker-compose.yml` for local ASP.NET Core and PostgreSQL execution using the confirmed ports, local environment-variable names, persistent volume, database healthcheck, and startup migrations. | IMP-001, IMP-002 | PENDING |
| IMP-004 | Build the shared Bootstrap 5 responsive shell: header, collapsible navigation, content area, footer, and confirmed color system. | IMP-001 | DONE |
| IMP-021 | Implement local username/password authentication, authenticated sessions, and the required first-login password change flow. | IMP-002; DG-014 | BLOCKED |
| IMP-022 | Provision the initial Administrator from configuration supplied outside the source code and require a password change at first login. | IMP-021; DG-014 | BLOCKED |
| IMP-020 | Manage system-user accounts: create/edit accounts with username, display name, and email attribute; assign one of the three fixed roles; activate/inactivate accounts; generate a temporary password shown once; and require its change at first login. Password reset/recovery and role-definition management are excluded from this delivery. | IMP-022; DG-014 | BLOCKED |

## Phase 2 — Business backlog: patient and responsible-person features

| ID | Feature | Depends on | Status |
|---|---|---|---|
| IMP-005 | Implement patient record creation with full name, age, and the specified initial CID behavior. | IMP-002; DG-001, DG-002 | BLOCKED |
| IMP-016 | Record optional dated anamneses for patients, including the confirmed sections, separate entries, and preserved revision history. | IMP-005; DG-008 | BLOCKED |
| IMP-006 | Register a responsible person with full name and degree of kinship and associate them with a patient. | IMP-005; DG-003 | BLOCKED |
| IMP-007 | Register telephone and email contact details for a responsible person. | IMP-006; DG-004 | BLOCKED |

## Phase 3 — Business backlog: professional, scheduling, and attendance features

| ID | Feature | Depends on | Status |
|---|---|---|---|
| IMP-008 | Register a fonoaudiólogo with confirmed basic data, unique CPF, certifications, and telephone/email contacts. | IMP-002; DG-005 | BLOCKED |
| IMP-015 | Allow association of one or more existing, active professionals during initial patient registration, later through the patient record, and through scheduling when no active association exists; support each professional being associated with multiple patients. | IMP-005, IMP-008 | BLOCKED |
| IMP-017 | Schedule and maintain patient attendances with required scheduled date/time and a fixed duration option per attendance, patient/professional overlap prevention, scheduling-flow association creation when needed, status transitions, actual realization timestamps, late indication, rescheduling with inactive-participant removal and a new duration selection, cancellation, and one current evolution entry per professional. | IMP-015, IMP-018; DG-009, DG-012 | BLOCKED |
| IMP-018 | Provide configurable cancellation-reason maintenance with create, list, edit, and inactivate operations; exclude the fixed `Remarcação` system reason, which is assigned automatically during rescheduling. | IMP-002; DG-012 | BLOCKED |
| IMP-019 | Remove patient-professional associations without deleting or altering existing attendance/evolution records or changing already scheduled attendance participants. | IMP-015, IMP-017 | BLOCKED |
| IMP-009 | Modify a professional, including the confirmed CPF uniqueness and certification/contact maintenance rules. | IMP-008; DG-005 | BLOCKED |
| IMP-010 | Inactivate/reactivate a professional, preserve patient associations during inactivity, apply the confirmed scheduling restriction, and preserve existing attendance participation/evolution rights. | IMP-008 | BLOCKED |
| IMP-011 | Delete a professional only when no patient is associated, using the confirmed deletion semantics. | IMP-008; DG-007 | BLOCKED |

## Phase 4 — Cross-cutting completion for the business backlog

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-012 | Apply confirmed authentication and authorization rules to implemented business operations. | IMP-005–IMP-011, IMP-015–IMP-022; DG-008, DG-009 | BLOCKED |
| IMP-013 | Verify implemented screens on desktop, tablet, and smartphone, including the shared layout and confirmed color system. | IMP-004–IMP-011, IMP-015–IMP-022 | BLOCKED |
| IMP-014 | Run the relevant automated tests and project checks for the approved implementation scope; record the results with the delivery. | IMP-001–IMP-013, IMP-015–IMP-022, IMP-024 | BLOCKED |

## Phase 5 — Deferred account-management backlog

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-024 | Add the Administrator-initiated password reset/recovery capability for system-user accounts. | IMP-020; DG-015 | BLOCKED |

## Deferred or excluded from the current roadmap

Do not implement these unless they are specified and approved as a future scope change:

- Public API retrieval of patient CID.
- Public API retrieval of professional council-registration information.
- Billing and other features not confirmed in the current scope.
