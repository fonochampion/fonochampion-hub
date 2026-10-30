# Requirements

**Status:** Draft
**Last reviewed:** 2026-10-30

## Functional requirements

Record each confirmed requirement with a stable identifier and observable acceptance criteria.

| ID | Requisito | Critérios de aceite | Status |
|---|---|---|---|
| FR-001 | Register a patient with full name, age, and associations to professionals who will attend the patient. | A system user can create a patient record with full name and age and may associate zero or more already registered, active professionals during registration. An active, registered professional may also be added later through the patient record. If no active professional is associated when scheduling an attendance, the scheduling flow offers the active registry and creates patient-professional associations for the selected professionals. Each professional may be associated with multiple patients. An association may be removed without deleting or altering existing attendance/evolution records; already scheduled attendances and their participants remain unchanged. Age representation, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-002 | Associate a list of responsible persons with a patient. | A patient record can be associated with responsible persons; each responsible person has a full name and a degree of kinship. The kinship domain, cardinality constraints, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-003 | Register telephone and email contact details for a responsible person. | A responsible person can have telephone and email contact details. Requiredness, multiplicity, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-004 | Record the patient's CID (International Classification of Diseases). | CID is a patient attribute. Retrieval from a public API is planned for a later stage; the API, integration behavior, and whether CID is available before that stage remain to be defined. | Confirmed; integration deferred |
| FR-005 | Register professionals, initially limited to fonoaudiólogos. | A system user can create a professional record with basic data, certifications, and contacts. The specific basic-data fields and the meaning of extensibility to other professions remain open. | Confirmed; details open |
| FR-006 | Record professional certifications. | Each certification contains a description, an alphanumeric registration, and a completion date. Whether certifications are required or multiple certifications are allowed remains open. | Confirmed; details open |
| FR-007 | Record professional contacts. | Professional contact types include telephone and email. Requiredness, multiplicity, and validation remain open. | Confirmed; details open |
| FR-008 | Modify a professional record. | A system user can alter an existing professional record. Editable fields and validation remain open; a CPF already belonging to another professional must not be accepted. | Confirmed; details open |
| FR-009 | Inactivate a professional. | A system user can inactivate a professional record. Existing patient associations are retained; the professional cannot be used for new scheduling while inactive. Reactivation restores the retained associations automatically. Inactivation does not alter existing attendance participation; the professional may continue participating and record/update their own evolution under the normal attendance-status rules. | Confirmed |
| FR-010 | Delete a professional only when no patient is associated with that professional. | The system prevents deletion while at least one patient is associated with the professional. A professional may be associated with multiple patients. Whether deletion is physical or logical remains open. | Confirmed; details open |
| FR-011 | Retrieve professional council-registration information from a public API. | This integration is planned for a later stage and is not part of the current implementation scope. API provider, data, and behavior remain open. | Confirmed; deferred |
| FR-012 | Record and maintain a patient's dated anamnesis history. | Patient registration may omit anamnesis. When provided, an anamnesis has a required performance date and the confirmed sections listed in [UC-001](./05-use-cases.md#uc-001--register-a-patient-and-record-anamnesis); all other fields are optional and may combine structured values with free-text observations. Each new anamnesis is a separate record; editing an existing record preserves prior versions and change history. Detailed prompts, version metadata, and access/privacy rules remain open. | Confirmed; details open |
| FR-013 | Schedule and maintain patient attendances and per-professional evolution entries. | Each attendance is scheduled with a date/time, a duration selected from fixed options of 10, 20, 30, 40, 50, or 60 minutes, and one or more active professionals associated with the patient. Any system user with scheduling permission may schedule or reschedule; which roles have that permission remains open. If no active professional is associated, scheduling offers the active registry and creates patient-professional associations for selected professionals. Any participating professional may start an `Agendado` attendance, end an `Em andamento` attendance, or cancel an `Agendado` attendance, subject to the conditions in [UC-008](./05-use-cases.md#uc-008--record-and-maintain-a-patient-attendance). Additional professionals may be added in any status except `Cancelado`. A participant may be removed only while `Agendado`, only if another participant remains, and only if the removed professional has no evolution. Each participating professional may record or replace only their own optional evolution entry in allowed statuses; prior versions are not retained, and missing entries remain individually indicated. Rescheduling cancels the existing scheduled attendance and creates a replacement for the same patient; it keeps existing active participants, removes inactive participants, and selects replacement professionals if no active participant remains. If none are actively associated, the scheduling flow offers all active professionals and creates the selected association(s). The replacement is linked to the prior record, assigned the fixed `Remarcação` reason automatically, and given a new duration selection. Attendance records and evolution entries cannot be deleted. No other attendance data may be edited. Access/privacy rules and remaining cancellation-reason maintenance details remain open. | Confirmed; details open |

The initial functional scope includes patient/responsible-person/contact registration, dated anamnesis records, scheduled and realized attendance records, and professional registration/maintenance described in [05 — Use Cases](./05-use-cases.md). The professional-council API is deferred. Do not infer billing or other features from the product description.

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
- Professional basic-data fields, CPF requiredness/validation, certification requiredness/multiplicity, and contact requiredness/multiplicity remain open.
- Professional associations are optional during patient registration; active, registered professionals may be added later through the patient record or, when none are actively associated, selected and associated as part of scheduling. Each attendance must have one or more active associated professionals when scheduling completes. An association may be removed without deleting or altering existing attendance/evolution records; already scheduled attendances and their participants remain unchanged. Professional inactivation retains patient associations, prevents new scheduling involving the inactive professional, and reactivation restores the associations. Inactivation does not alter existing attendance participation or prevent otherwise-allowed evolution updates.
- Anamnesis records are optional during patient registration, have a required performance date, and preserve previous records and revisions. Detailed prompts, revision metadata, access controls, privacy, and retention remain open.
- Attendance records concern a patient and one or more associated active professionals and always include a scheduled date/time and a duration selected from fixed options of 10, 20, 30, 40, 50, or 60 minutes; attendance records and evolution entries cannot be deleted. Each participating professional has one optional current evolution entry and may maintain only their own entry. See UC-008 for status transitions, cancellation, rescheduling, late indication, and actual realization timestamps. Cancellation-reason maintenance details and access/privacy rules remain open.
- User roles, authentication, and authorization remain open.
- Data classification, consent/legal basis, and access controls remain open. Retention and deletion rules for data other than attendance/evolution records remain open; those records are retained permanently and cannot be deleted. See [04 — Data and Integrations](./04-data-and-integrations.md).
