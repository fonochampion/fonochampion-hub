# ADR-009: Patient Anamnesis Records

**Status:** Accepted
**Date:** 2026-10-29

## Context

Patient registration needs to support recording speech-language therapy anamnesis while preserving the patient's history as information changes over time. Anamnesis contains clinical and health information, so its access and handling rules must not be inferred.

## Options considered

1. Require one anamnesis as part of patient registration.
2. Allow patient registration without anamnesis and record dated anamneses separately over time.

## Decision

- Anamnesis is optional during patient registration.
- Each anamnesis has a required date indicating when it was performed. All other fields are optional.
- The initial sections are: main complaint and reason for seeking care; health history, relevant conditions, medications, and previous treatments; development history and relevant background; communication, speech, language, and learning; hearing and hearing history; voice; orofacial motor function, feeding, and swallowing; school, work, and social context; family context and patient/responsible-person expectations; and clinical observations.
- Anamnesis may combine structured fields with free-text observations.
- Each anamnesis is a separate dated record. Creating a new record does not overwrite earlier records.
- Editing an existing record preserves previous versions and change history.
- Detailed prompts or value sets, revision-history metadata, privacy, and retention remain open. Role-based access is specified by ADR-018; applicable legal and data-handling constraints remain open.

Detailed flows and acceptance criteria are in [05 — Use Cases](../05-use-cases.md).

## Consequences

- **Benefits:** Supports a changing clinical history without replacing earlier anamnesis records and permits patient registration before an anamnesis is available.
- **Costs or risks:** These records contain health information. Access, privacy, retention, and revision audit requirements must be defined before the affected functionality is implemented.
- **Follow-up actions:** Resolve the open clinical-data and access decisions before implementation; see [06 — Implementation Roadmap](../06-implementation-checklist.md).
