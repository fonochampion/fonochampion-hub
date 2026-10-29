# Data and Integrations

**Status:** Draft
**Last reviewed:** 2026-10-29

## Data

- **Required entities and relationships:** Patient (full name, age, CID) with responsible person(s) (full name, degree of kinship), their contact details (telephone, email), optional dated anamnesis records with preserved revisions, and completed attendance records. An attendance concerns one patient and one or more active professionals already associated with that patient; each participating professional has an optional evolution entry with preserved revisions. A patient must have at least one associated active professional before an attendance can be recorded. Active professionals may be added to a patient through the patient record after initial registration; whether associations may be removed remains open. Until scheduling is available, attendance date/time may be absent; future attendance records must link to an appointment and obtain date/time from it. Patient records may optionally be associated during registration with one or more existing, active professionals. A professional may be associated with multiple patients. Professional records are initially limited to fonoaudiólogos and include basic data, CPF, certifications, and telephone/email contacts. Certification fields are description, alphanumeric registration, and completion date. See [05 — Use Cases](./05-use-cases.md).
- **Professional CPF:** CPF defines professional uniqueness. A professional CPF must not duplicate another professional's CPF. Whether CPF is mandatory and how it is formatted/validated remain open.
- **Data sources and ownership:** To be defined
- **Personal or sensitive data:** Patient and responsible-person records include names, age, CID, kinship, telephone, and email. Anamnesis and attendance evolution contain patient health and clinical information. Professional records include basic personal data, CPF, certification details, and contact information. Applicable handling requirements and classification remain to be defined.
- **Retention, deletion, and export:** To be defined

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
- Appointment scheduling is deferred. Whether clinical session data beyond the evolution defined in UC-008 is in scope remains to be defined.
- Define professional basic-data fields and CPF requiredness and validation.
- Define certification and professional-contact requiredness, multiplicity, and validation.
- Define whether professional associations can be removed after patient registration and how inactivation affects existing associations.
- Define access controls, privacy, retention, and audit requirements for anamnesis records, including which users may view or edit them.
- Define detailed anamnesis prompts or value sets and which metadata is retained for revisions.
- Define access controls, privacy, retention, and audit requirements for attendance and evolution records.
- Define attendance/evolution deletion or cancellation behavior and the metadata retained for evolution revisions.
- Define the effect on historical attendance records if a patient-professional association is removed.
- Define the future scheduling model required to supply date/time and link every attendance to an appointment.
- Define effects of professional inactivation, reactivation behavior, and deletion semantics.
- Select and assess a public professional-council API before implementing that later-stage integration; no provider has been selected.
