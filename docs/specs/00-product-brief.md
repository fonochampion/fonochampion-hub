# Product Brief

**Status:** Draft
**Last reviewed:** 2026-10-31

## Context

- **Product name:** To be defined
- **One-sentence description:** A system for managing therapy sessions, initially focused on speech-language therapy.
- **Problem motivating the product:** To be defined
- **How this problem is addressed today:** To be defined

## Users

- **Primary audience:** System administrators, attendants, and therapists, with responsibilities defined in [05 — Use Cases](./05-use-cases.md).
- **Needs and pain points:** To be defined
- **Secondary audiences:** To be defined

## Goals and success

- **Expected outcomes:** To be defined
- **How success will be measured:** To be defined
- **Quantitative targets or constraints:** To be defined

## Scope

- **Included in the initial stage:** Register patients with responsible persons and their contacts; optionally associate one or more already registered, active fonoaudiólogos during patient registration and allow their later association through the patient record or scheduling flow when no active association exists; optionally record dated anamneses with preserved history; schedule and record patient attendances with their scheduled and actual realization periods, defined status flow, and one optional current evolution per participating professional; register and maintain professionals with basic data, certifications, and contacts; manage system-user accounts and assign exactly one of the defined roles. A professional may be associated with multiple patients. See [05 — Use Cases](./05-use-cases.md).
- **First implementation delivery:** Establish the confirmed application foundation and deliver local username/password authentication, initial Administrator provisioning through external configuration, and system-user account management using the three fixed roles. Accounts have a username, display name, and email attribute; email is not used to authenticate. The system generates a temporary password for accounts created by an Administrator, displays it once, and requires a password change at first login. Password reset is deferred from this delivery. This delivery sequence does not remove the remaining confirmed product scope; patient, professional, anamnesis, attendance, and evolution features remain in the implementation backlog.
- **Explicitly out of scope:** To be defined
- **Known dependencies and constraints:** To be defined

## Assumptions and open questions

- **Assumptions to validate:** Patient age representation, CID behavior before its future public API integration, professional basic-data fields, and remaining validation/cardinality rules remain open. Anamnesis section details, revision metadata, privacy, and retention remain open; role access is defined in UC-009. Attendance/evolution privacy and legal/data-handling requirements and remaining cancellation-reason values remain open. Attendance/evolution records are retained permanently; specified audit metadata and the fixed per-attendance duration options are defined. Association removal and professional inactivation preserve existing attendance participation as specified in UC-008. Attendance scheduling is included in the initial product scope; scheduled date/time are required, and actual start/end times are recorded when the professional changes the attendance state. Professional registration is initially limited to fonoaudiólogos; support for other professions may be considered later. Password reset mechanics and account deletion remain open; initial username/password, account-field, session, failed-login, and bootstrap policies are defined in UC-009 and ADR-022/ADR-023/ADR-024.
- **Outstanding questions:** Product name, success measures, detailed first-release scope beyond the initial registration stage, and how the product currently addresses the problem.
