# Data and Integrations

**Status:** Draft
**Last reviewed:** 2026-10-30

## Data

- **Required entities and relationships:** Patient (full name, age, CID) with responsible person(s) (full name, degree of kinship), their contact details (telephone, email), optional dated anamnesis records with preserved revisions, and scheduled attendance records. Each attendance concerns one patient and one or more professionals who are active and associated with the patient when scheduling completes. Every attendance has a required scheduled date/time and automatically recorded actual start/end date/time; its statuses and rescheduling relationship are defined in UC-008. Each participating professional has one optional current evolution entry with separate therapeutic-plan and observation fields; the plan is required, observations are optional, updates replace the current content without preserving prior versions, and a professional may record or update only their own entry while the attendance is `Em andamento` or `Aguardando Evolução`. The system records the evolution's creator/creation time and last updater/last update time. Permissions for other user roles remain open. If no active professional is associated when scheduling, the flow offers the active professional registry and creates patient-professional associations for selected professionals. Active professionals may also be added to a patient through the patient record after initial registration; associations may be removed without deleting or altering existing attendance/evolution records, and already scheduled attendances and their participants remain unchanged. Patient records may optionally be associated during registration with one or more existing, active professionals. A professional may be associated with multiple patients. Professional records are initially limited to fonoaudiólogos and include basic data, CPF, certifications, and telephone/email contacts. Certification fields are description, alphanumeric registration, and completion date. See [05 — Use Cases](./05-use-cases.md).
- **Attendance status history:** The system preserves each status change with its author and date/time. For an automatic transition, the author is the professional or user whose action triggered it.
- **Appointment duration:** Each attendance has a duration selected from the fixed options of 10, 20, 30, 40, 50, or 60 minutes. The duration is used with the scheduled date/time to prevent overlaps. A replacement created by rescheduling requires a new duration selection.
- **Professional CPF:** CPF defines professional uniqueness. A professional CPF must not duplicate another professional's CPF. Whether CPF is mandatory and how it is formatted/validated remain open.
- **Data sources and ownership:** To be defined
- **Personal or sensitive data:** Patient and responsible-person records include names, age, CID, kinship, telephone, and email. Anamnesis and attendance evolution contain patient health and clinical information. Professional records include basic personal data, CPF, certification details, and contact information. Applicable handling requirements and classification remain to be defined.
- **Retention, deletion, and export:** Attendance records and evolution entries are retained permanently and cannot be deleted. Retention and deletion rules for other data, including anamneses, remain to be defined.

## External integrations

| Service | Purpose | Data shared | Status |
|---|---|---|---|
| Public CID API | Retrieve a patient's CID | To be defined | Planned for a later stage; provider and integration details open |
| Public professional-council API | Retrieve professional council-registration information | To be defined | Planned for a later stage; provider and integration details open |

## Privacy and security

- **Consent and legal basis:** To be defined
- **Access controls:** To be defined
- **Regulatory requirements:** To be defined

## Assumptions and open questions

- Define age representation, CID handling before API integration, kinship domain values, and cardinality for patient/responsible-person/contact relationships.
- Define contact requiredness, multiplicity, and validation for telephone and email.
- Select and assess the public CID API before implementing that later-stage integration; no provider has been selected.
- Define duplicate handling, record updates, and deletion or archival behavior.
- Define user roles, access controls, consent/legal basis, data protection requirements, and retention.
- Appointment scheduling is included in the current scope. Any system user with scheduling permission may schedule or reschedule; the roles receiving this permission remain open. If the patient has no active associated professional, the scheduling flow offers all active professionals and creates associations for the selection. Each attendance has a duration selected from the fixed options of 10, 20, 30, 40, 50, or 60 minutes and used with its scheduled date/time to prevent overlaps. A professional's existing `Agendado` attendances prevent overlap; other attendance statuses do not count as scheduling conflicts. Overlapping attendances for the same patient are prevented.
- Define professional basic-data fields and CPF requiredness and validation.
- Define certification and professional-contact requiredness, multiplicity, and validation.
- Professional associations may be removed without deleting or altering existing attendance/evolution records; already scheduled attendances and their participants remain unchanged. Professional inactivation retains existing associations, prevents new scheduling involving the inactive professional, and reactivation restores the associations. Inactivation does not alter existing attendance participation or prevent otherwise-allowed evolution updates. Rescheduling removes inactive participants; if none remain, the flow selects active associated professionals or, if none are associated, creates associations from the active professional registry.
- Define access controls, privacy, retention, and audit requirements for anamnesis records, including which users may view or edit them.
- Define detailed anamnesis prompts or value sets and which metadata is retained for revisions.
- Attendance and evolution records are retained permanently and cannot be deleted. Define access controls and privacy requirements for these records.
- Define permissions to maintain cancellation reasons and the remaining configurable values. Configurable reasons have a required unique name (unique across active and inactive records), an optional description, and an active/inactive indicator; they support create, list, edit, and inactivate operations and cannot be deleted. An inactivated reason is unavailable for new cancellations but remains on historical attendances, and editing a reason does not alter the original name and description shown in historical attendances. `Remarcação` is a fixed system reason assigned automatically during rescheduling.
- Define professional deletion semantics.
- Select and assess a public professional-council API before implementing that later-stage integration; no provider has been selected.
