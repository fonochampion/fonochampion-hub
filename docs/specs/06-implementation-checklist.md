# Implementation Roadmap

**Status:** Working roadmap
**Last reviewed:** 2026-10-29

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

## Phase 0 — Resolve specification gates

These are product decisions, not implementation tasks. Resolve each in the specifications before implementing work that depends on it. Relevant items below remain `BLOCKED` until their gates are closed.

| Gate | Open decision | Source |
|---|---|---|
| DG-001 | Age representation and validation | [UC-001](./05-use-cases.md#uc-001--register-a-patient) |
| DG-002 | How CID is managed before the deferred public API integration | [UC-001](./05-use-cases.md#uc-001--register-a-patient) |
| DG-003 | Kinship values, patient/responsible-person cardinality, and duplicate handling | [UC-002](./05-use-cases.md#uc-002--register-and-associate-a-responsible-person) |
| DG-004 | Contact requiredness, multiplicity, and validation | [UC-003](./05-use-cases.md#uc-003--register-contact-methods-for-a-responsible-person) |
| DG-005 | Professional basic-data fields, CPF requiredness/validation, certification and contact rules, and editable fields | [UC-004–UC-005](./05-use-cases.md#use-cases) |
| DG-006 | Authentication, user roles, and authorization | [Requirements](./01-requirements.md#assumptions-and-open-questions) |
| DG-007 | Effects of inactivation on existing patient associations and professional deletion semantics | [UC-006–UC-007](./05-use-cases.md#use-cases) |
| DG-008 | Anamnesis access controls, privacy/retention, detailed section prompts, and revision metadata | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [Data and Integrations](./04-data-and-integrations.md#privacy-and-security) |
| DG-009 | Attendance/evolution access controls, privacy/retention, and revision metadata | [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance), [Data and Integrations](./04-data-and-integrations.md#privacy-and-security) |
| DG-010 | Effects of removing a patient-professional association on existing attendance records and evolutions | [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis), [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance) |

## Phase 1 — Application foundation

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-001 | Create the .NET solution and confirmed `src/Web`, `src/Application`, `src/Domain`, and `src/Infrastructure` project structure. | — | PENDING |
| IMP-002 | Configure ASP.NET Core MVC, Entity Framework Core, Npgsql, and PostgreSQL persistence in the confirmed modular-monolith structure. | IMP-001 | PENDING |
| IMP-003 | Add the root `Dockerfile` and `docker-compose.yml` for local ASP.NET Core and PostgreSQL execution. | IMP-001, IMP-002 | PENDING |
| IMP-004 | Build the shared Bootstrap 5 responsive shell: header, collapsible navigation, content area, footer, and confirmed color system. | IMP-001 | PENDING |

## Phase 2 — Patient and responsible-person features

| ID | Feature | Depends on | Status |
|---|---|---|---|
| IMP-005 | Implement patient record creation with full name, age, and the specified initial CID behavior. | IMP-002; DG-001, DG-002 | BLOCKED |
| IMP-016 | Record optional dated anamneses for patients, including the confirmed sections, separate entries, and preserved revision history. | IMP-005; DG-008 | BLOCKED |
| IMP-006 | Register a responsible person with full name and degree of kinship and associate them with a patient. | IMP-005; DG-003 | BLOCKED |
| IMP-007 | Register telephone and email contact details for a responsible person. | IMP-006; DG-004 | BLOCKED |

## Phase 3 — Professional and attendance features

| ID | Feature | Depends on | Status |
|---|---|---|---|
| IMP-008 | Register a fonoaudiólogo with confirmed basic data, unique CPF, certifications, and telephone/email contacts. | IMP-002; DG-005 | BLOCKED |
| IMP-015 | Allow association of one or more existing, active professionals during initial patient registration and later through the patient record; support each professional being associated with multiple patients. | IMP-005, IMP-008 | BLOCKED |
| IMP-017 | Record completed patient attendances only when the patient already has at least one associated active professional; support adding more associated active professionals after saving and optional per-professional evolution entries with preserved revisions. | IMP-015; DG-009 | BLOCKED |
| IMP-009 | Modify a professional, including the confirmed CPF uniqueness and certification/contact maintenance rules. | IMP-008; DG-005 | BLOCKED |
| IMP-010 | Inactivate a professional and apply the confirmed effects to its patient associations. | IMP-008; DG-007 | BLOCKED |
| IMP-011 | Delete a professional only when no patient is associated, using the confirmed deletion semantics. | IMP-008; DG-007 | BLOCKED |

## Phase 4 — Cross-cutting completion

| ID | Implementation item | Depends on | Status |
|---|---|---|---|
| IMP-012 | Apply confirmed authentication and authorization rules to implemented operations. | IMP-005–IMP-011, IMP-015–IMP-017; DG-006 | BLOCKED |
| IMP-013 | Verify implemented screens on desktop, tablet, and smartphone, including the shared layout and confirmed visual system. | IMP-004–IMP-011, IMP-015–IMP-017 | BLOCKED |
| IMP-014 | Run the relevant automated tests and project checks for the approved implementation scope; record the results with the delivery. | IMP-001–IMP-013, IMP-015–IMP-017 | BLOCKED |

## Deferred or excluded from the current roadmap

Do not implement these unless they are specified and approved as a future scope change:

- Public API retrieval of patient CID.
- Public API retrieval of professional council-registration information.
- Appointment scheduling and its lifecycle, billing, and other features not confirmed in the current scope. Attendance recording is specified in [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance); when scheduling is later introduced, its relationship to attendance records must follow the confirmed requirement.
