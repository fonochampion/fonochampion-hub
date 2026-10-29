# Product Brief

**Status:** Draft
**Last reviewed:** 2026-10-29

## Context

- **Product name:** To be defined
- **One-sentence description:** A system for managing therapy sessions, initially focused on speech-language therapy.
- **Problem motivating the product:** To be defined
- **How this problem is addressed today:** To be defined

## Users

- **Primary audience:** To be defined; specific user roles are open.
- **Needs and pain points:** To be defined
- **Secondary audiences:** To be defined

## Goals and success

- **Expected outcomes:** To be defined
- **How success will be measured:** To be defined
- **Quantitative targets or constraints:** To be defined

## Scope

- **Included in the initial stage:** Register patients with responsible persons and their contacts; optionally associate one or more already registered, active fonoaudiólogos during patient registration and allow their later association through the patient record; optionally record dated anamneses with preserved history; record completed patient attendances only when the patient already has at least one associated active professional, and each participating professional's optional evolution with preserved revisions; register and maintain professionals with basic data, certifications, and contacts. A professional may be associated with multiple patients. Scheduling is deferred. See [05 — Use Cases](./05-use-cases.md).
- **Explicitly out of scope:** To be defined
- **Known dependencies and constraints:** To be defined

## Assumptions and open questions

- **Assumptions to validate:** Patient age representation, CID behavior before its future public API integration, professional basic-data fields, and remaining validation/cardinality rules remain open. Anamnesis section details, revision metadata, access controls, privacy, and retention remain open. Attendance/evolution access controls, privacy, retention, revision metadata, deletion/cancellation, the effect of removing a patient-professional association on attendance history, and the effects of professional inactivation remain open. Attendance date and time will come from a future scheduling capability; until then they may be absent. Professional registration is initially limited to fonoaudiólogos; support for other professions may be considered later.
- **Outstanding questions:** Product name, user roles, success measures, detailed first-release scope beyond the initial registration stage, and how the product currently addresses the problem.
