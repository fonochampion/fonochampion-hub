# Requirements

**Status:** Draft
**Last reviewed:** 2026-10-29

## Functional requirements

Record each confirmed requirement with a stable identifier and observable acceptance criteria.

| ID | Requisito | Critérios de aceite | Status |
|---|---|---|---|
| FR-001 | Register a patient with full name, age, and associations to professionals who will attend the patient. | A system user can create a patient record with full name and age and may associate zero or more already registered, active professionals during registration. An active, registered professional may also be added later through the patient record. Each professional may be associated with multiple patients. A patient may exist without an associated professional, but an attendance cannot be recorded until at least one active professional is associated. Whether an association can be removed, age representation, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-002 | Associate a list of responsible persons with a patient. | A patient record can be associated with responsible persons; each responsible person has a full name and a degree of kinship. The kinship domain, cardinality constraints, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-003 | Register telephone and email contact details for a responsible person. | A responsible person can have telephone and email contact details. Requiredness, multiplicity, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-004 | Record the patient's CID (International Classification of Diseases). | CID is a patient attribute. Retrieval from a public API is planned for a later stage; the API, integration behavior, and whether CID is available before that stage remain to be defined. | Confirmed; integration deferred |
| FR-005 | Register professionals, initially limited to fonoaudiólogos. | A system user can create a professional record with basic data, certifications, and contacts. The specific basic-data fields and the meaning of extensibility to other professions remain open. | Confirmed; details open |
| FR-006 | Record professional certifications. | Each certification contains a description, an alphanumeric registration, and a completion date. Whether certifications are required or multiple certifications are allowed remains open. | Confirmed; details open |
| FR-007 | Record professional contacts. | Professional contact types include telephone and email. Requiredness, multiplicity, and validation remain open. | Confirmed; details open |
| FR-008 | Modify a professional record. | A system user can alter an existing professional record. Editable fields and validation remain open; a CPF already belonging to another professional must not be accepted. | Confirmed; details open |
| FR-009 | Inactivate a professional. | A system user can inactivate a professional record. Effects of inactivation and whether reactivation is supported remain open. | Confirmed; details open |
| FR-010 | Delete a professional only when no patient is associated with that professional. | The system prevents deletion while at least one patient is associated with the professional. A professional may be associated with multiple patients. Whether deletion is physical or logical remains open. | Confirmed; details open |
| FR-011 | Retrieve professional council-registration information from a public API. | This integration is planned for a later stage and is not part of the current implementation scope. API provider, data, and behavior remain open. | Confirmed; deferred |
| FR-012 | Record and maintain a patient's dated anamnesis history. | Patient registration may omit anamnesis. When provided, an anamnesis has a required performance date and the confirmed sections listed in [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis); all other fields are optional and may combine structured values with free-text observations. Each new anamnesis is a separate record; editing an existing record preserves prior versions and change history. Detailed prompts, version metadata, and access/privacy rules remain open. | Confirmed; details open |
| FR-013 | Record completed patient attendances and maintain per-professional evolution entries. | An attendance concerns one patient and one or more active professionals already associated with that patient. A patient must have at least one associated active professional before an attendance can be recorded. Each participating professional has an optional evolution entry summarizing the therapeutic plan carried out and observations; an attending professional may record or update entries, with prior versions preserved. Evolution may remain unrecorded and is visibly indicated as such. The defined maintenance operations are adding participating professionals and recording/updating evolution entries; removing professionals and editing other attendance data remain unspecified. Until scheduling exists, date and time may be absent; when scheduling is introduced, every attendance must link to an appointment and take its date/time from it. Attendance/evolution deletion or cancellation, revision metadata, and access/privacy rules remain open. | Confirmed; details open |

The initial functional scope is limited to patient/responsible-person/contact registration, dated anamnesis records, completed attendance records, and professional registration/maintenance described in [05 — Use Cases](./05-use-cases.md). Appointment scheduling and the professional-council API are deferred. Do not infer billing or other features from the product description.

## Non-functional requirements

Include only necessary, confirmed requirements, such as performance, availability, security, privacy, compatibility, and accessibility. Use verifiable metrics where possible.

| ID | Category | Requirement and measure | Status |
|---|---|---|---|
| NFR-001 | Architecture | The application must use a modular monolith. Acceptance: deliver the application as a single deployable system with modules, not as independently deployed microservices. | Confirmed |
| NFR-002 | Operational cost | Prefer low operational cost and avoid unnecessary infrastructure. Acceptance: each additional operational component is justified by an explicit, confirmed requirement; do not add components for hypothetical scale. | Confirmed |
| NFR-003 | Maintainability and evolution | Favor simple, maintainable solutions that support incremental evolution. Acceptance: proposed changes use the confirmed project structure and introduce no architectural component or abstraction without a demonstrated requirement. | Confirmed |
| NFR-004 | Responsiveness | The user interface must support desktop, tablet, and smartphone viewports using the confirmed standard layout. Acceptance: validate defined primary screens in all three device classes, with usable navigation and no loss of the header, content area, or footer. | Confirmed |
| NFR-005 | Local development | Provide a Docker-based local environment for the ASP.NET Core application and PostgreSQL. Acceptance: the repository includes `Dockerfile` and `docker-compose.yml` configured to run both services locally. | Confirmed |

## Constraints

- **Regulatory or contractual:** To be defined
- **Operational or business:** To be defined
- **Platforms and browsers:** To be defined

## Out of scope

- To be defined

## Assumptions and open questions

- Age representation, CID availability before the future API integration, kinship domain values, validation, duplicate handling, and create/read/update/delete lifecycle for these records remain open.
- Contact requiredness and multiplicity remain open.
- Professional basic-data fields, CPF requiredness/validation, certification requiredness/multiplicity, contact requiredness/multiplicity, and inactivation effects remain open.
- Professional associations are optional during patient registration; active, registered professionals may be added later through the patient record. At least one active association is required before recording an attendance. Association removal and the effects of inactivation on existing associations remain open.
- Anamnesis records are optional during patient registration, have a required performance date, and preserve previous records and revisions. Detailed prompts, revision metadata, access controls, privacy, and retention remain open.
- Attendance records concern a patient and one or more associated active professionals. Each participating professional has an optional evolution with preserved revisions; date and time can be absent until scheduling is available, after which an appointment link is mandatory. Attendance/evolution deletion or cancellation, revision metadata, and access/privacy rules remain open.
- User roles, authentication, and authorization remain open.
- Data classification, consent/legal basis, access controls, retention, and deletion requirements remain open; see [04 — Data and Integrations](./04-data-and-integrations.md).
