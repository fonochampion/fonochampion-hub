# Initial-Stage Use Cases

**Status:** Draft
**Last reviewed:** 2026-10-29

## Scope

The product is a system for therapy sessions, initially focused on speech-language therapy. The confirmed initial stage covers registration of patients, their responsible persons and contact methods, patient anamnesis, records of completed patient attendances, and professionals initially limited to fonoaudiólogos. Scheduling is not part of the current stage.

The confirmed relationships and patient information are:

```text
Patient > Responsible person(s) > Contact method(s)
Patient > zero or more active, registered professionals
Professional > zero or more patients
Patient > dated anamnesis records > preserved revisions
Patient > completed attendances > one or more participating professionals
Attendance > one evolution entry per participating professional > preserved revisions
```

The patient/responsible-person hierarchy and the attributes listed in the use cases are confirmed. During patient registration, the system user may associate zero or more already registered and active professionals with the patient; professionals may also be added later through the patient record. Each professional may be associated with multiple patients. A patient may be registered without an anamnesis. Anamneses are separate, dated records; creating a new record does not overwrite previous records, and editing a record preserves its earlier versions. A completed attendance concerns one patient and one or more active professionals already associated with that patient. A patient must have at least one associated professional before an attendance can be recorded; an attendance cannot itself establish the patient-professional association. Each participating professional has a separate, optional evolution entry; an attending professional may record or update evolution entries, and changes preserve prior versions. Scheduling is deferred; current attendance records may have no date or time until the future scheduling capability is available, at which point each attendance must be linked to an appointment and receive its date and time from it. Age representation, CID availability before its planned API integration, kinship values, responsible-person/contact cardinality, requiredness, validation, clinical-data access controls, retention, attendance deletion/cancellation, and patient-record lifecycle remain open. Whether professional associations can be removed from a patient, and the effects of professional inactivation on existing associations, remain open. Professional council-registration information will be retrieved from a public API in a later stage. Do not infer additional product features such as billing.

## Actors

- **System user:** Performs registration. Specific user roles, authentication, and permissions remain to be defined.
- **Attending professional:** Participates in an attendance and may record or update its evolution entries. Access-control rules remain to be defined.

## Use cases

### UC-001 — Register a patient and record anamnesis

**Status:** Confirmed; details open
**Goal:** Create a patient record with full name and age, a CID attribute whose retrieval by a public API is planned for a later stage, optionally associate one or more already registered, active professionals who will attend the patient, and optionally record dated anamnesis information.

**Main flow:**

1. The system user starts patient registration.
2. The user provides the patient's full name and age.
3. The user may select zero or more already registered, active professionals to associate with the patient.
4. The user may add an anamnesis, including its date of completion and information in the confirmed anamnesis sections.
5. The system creates the patient record, records the selected professional associations, and stores the anamnesis as a dated record when provided.

**Acceptance criteria:**

- A patient record can be created.
- Full name and age are patient attributes.
- Associating a professional during patient registration is optional; the patient can be registered with no professional.
- The user can associate one or more already registered, active professionals during patient registration.
- A professional can be associated with multiple patients.
- An unregistered or inactive professional cannot be selected for a new association during patient registration.
- An active, registered professional may be added to a patient after initial registration through the patient record.
- Whether a professional association may be removed from a patient remains to be defined.
- Patient registration may be completed without an anamnesis.
- An anamnesis has a required date indicating when it was performed.
- The initial anamnesis sections are: main complaint and reason for seeking care; health history, relevant conditions, medications, and previous treatments; development history and relevant background; communication, speech, language, and learning; hearing and hearing history; voice; orofacial motor function, feeding, and swallowing; school, work, and social context; family context and patient/responsible-person expectations; and clinical observations.
- Anamnesis information may combine structured fields with free-text observations. All fields other than the performance date are optional.
- Each anamnesis is a separate dated record. Adding a new anamnesis does not overwrite previous records.
- Editing an existing anamnesis preserves its previous versions and change history.
- Detailed prompts or value sets within the anamnesis sections, version-history metadata, and access controls for clinical information remain to be defined.
- Whether age is entered directly or derived, and its validation, remain to be defined.
- CID is a patient attribute. Retrieval through a public API is planned for a later stage; the public API has not been selected.
- Whether CID can be manually entered or otherwise managed before that integration is available remains to be defined.
- Duplicate handling remains to be defined.

### UC-002 — Register and associate a responsible person

**Status:** Confirmed; details open
**Goal:** Record a responsible person's full name and degree of kinship, and associate that person with a patient.

**Precondition:** The patient exists or is being registered in the same flow; whether the same-flow option is supported remains open.

**Main flow:**

1. The system user selects or identifies the patient.
2. The user provides the responsible person's full name and degree of kinship.
3. The system records the responsible person associated with that patient.

**Acceptance criteria:**

- A responsible-person record can be associated with its patient.
- Whether a patient may have multiple responsible persons, or a responsible person may be associated with multiple patients, remains to be defined.
- The confirmed responsible-person attributes are full name and degree of kinship.
- The kinship domain values, validation, and duplicate handling remain to be defined.

### UC-003 — Register contact methods for a responsible person

**Status:** Confirmed; details open
**Goal:** Record telephone and email contact details associated with a responsible person.

**Precondition:** The responsible person exists or is being registered in the same flow; whether the same-flow option is supported remains open.

**Main flow:**

1. The system user selects or identifies the responsible person.
2. The user provides the person's telephone and email contact details.
3. The system records the contact information associated with that responsible person.

**Acceptance criteria:**

- Contact information can be associated with the relevant responsible-person record.
- The confirmed contact types are telephone and email.
- Requiredness, whether multiple values of either type are allowed, preferred-contact behavior, and validation remain to be defined.

### UC-004 — Register a professional

**Status:** Confirmed; details open
**Goal:** Register a professional, initially limited to fonoaudiólogos, with basic data, CPF, certifications, and contact information.

**Main flow:**

1. The system user starts professional registration.
2. The user enters the professional's basic data, including CPF.
3. The user records certification details (description, alphanumeric registration, and completion date); whether certifications may be omitted or multiple certifications may be recorded remains open.
4. The user records the professional's applicable contact details (telephone and/or email); requiredness and multiplicity remain open.
5. The system checks that the CPF does not belong to another professional.
6. The system creates the professional record if the CPF is not duplicated.

**Acceptance criteria:**

- The initial supported professional type is fonoaudiólogo. Support for other professional types is not part of the current scope.
- CPF defines professional uniqueness. A CPF that duplicates another professional's CPF is rejected.
- A certification has a description, an alphanumeric registration, and a completion date.
- Professional contact types include telephone and email.
- Professional basic-data fields beyond CPF, CPF requiredness and validation, certification requiredness/multiplicity, and contact requiredness/multiplicity and validation remain open.
- Professional-council registration data is planned to be retrieved from a public API in a later stage. No API is selected, and that integration is not part of this use case's current implementation.

### UC-005 — Modify a professional

**Status:** Confirmed; details open
**Goal:** Change an existing professional record.

**Main flow:**

1. The system user identifies an existing professional.
2. The user changes the professional's data.
3. If the CPF is changed, the system ensures it does not duplicate another professional's CPF.
4. The system saves the changes if the CPF is not duplicated.

**Acceptance criteria:**

- An existing professional record can be modified.
- A CPF already assigned to a different professional cannot be saved.
- Which fields can be changed and how fields, certifications, and contacts are individually maintained remain to be defined.

### UC-006 — Inactivate a professional

**Status:** Confirmed; details open
**Goal:** Inactivate an existing professional record.

**Main flow:**

1. The system user identifies an existing professional.
2. The user requests inactivation.
3. The system marks the professional as inactive.

**Acceptance criteria:**

- A professional can be inactivated.
- The effects of inactivation, including its impact on any patient association, and whether reactivation is supported remain to be defined.

### UC-007 — Delete a professional

**Status:** Confirmed; details open
**Goal:** Remove a professional record when no patient is associated with that professional.

**Main flow:**

1. The system user identifies a professional and requests deletion.
2. The system checks whether any patient is associated with the professional.
3. If a patient is associated, the system prevents deletion.
4. If no patient is associated, the system permits deletion.

**Acceptance criteria:**

- A professional associated with one or more patients cannot be deleted.
- Deletion is permitted only when no patient is associated with the professional.
- Whether deletion is physical or logical remains to be defined.

### UC-008 — Record and maintain a patient attendance

**Status:** Confirmed; details open
**Goal:** Record a completed attendance for a patient and one or more associated professionals, and record each participating professional's optional evolution.

**Preconditions:**

- The patient exists.
- At least one active, registered professional is associated with the patient before the attendance is recorded.

**Main flow:**

1. A system user starts a record for a completed attendance and identifies the patient.
2. The user selects one or more active, registered professionals already associated with that patient.
3. The system creates the attendance record for that patient and its participating professionals.
4. For each participating professional, the system indicates that the evolution has not yet been recorded.
5. After the attendance is saved, the system user may add further active, registered professionals already associated with the patient.
6. Any professional participating in the attendance may record or update evolution entries. Each entry summarizes the therapeutic plan carried out and includes the professional's observations during the attendance.
7. When an evolution entry is recorded or updated, the system preserves its previous versions.

**Acceptance criteria:**

- An attendance record is associated with exactly one patient and one or more participating professionals.
- Only active, registered professionals associated with the patient may be included in the attendance.
- An attendance cannot be recorded unless the patient already has at least one associated active professional. If the patient has none, the professional must first be added through the patient record.
- A professional may participate in attendances for multiple patients.
- Each participating professional has an individual evolution entry; the entry is optional.
- The system indicates when a professional's evolution has not yet been recorded. This state can later be updated when an evolution is entered.
- An evolution summarizes the therapeutic plan carried out and records observations made by the professional during the attendance.
- Any professional participating in the attendance may record or update evolution entries.
- Updating an evolution preserves its previous versions. Revision-history metadata remains to be defined.
- The defined maintenance operations are adding participating professionals and recording or updating evolution entries. Removing a participating professional and editing other attendance data remain unspecified.
- Additional participating professionals may be added after the attendance has been saved, provided they are active, registered, and already associated with the patient. Removal remains unspecified.
- Scheduling is not included in the current scope. Until scheduling is available, an attendance may be recorded without date or time. Once scheduling is available, every attendance must be linked to an appointment and its date and time must come from that appointment.
- Whether an attendance or evolution can be deleted or cancelled, clinical-data access and retention rules, and the precise revision-history metadata remain to be defined.
- Scheduling, appointments, and their lifecycle are separate future scope and are not implemented by this use case.

## Open questions

- Which user roles may perform these use cases, and what authentication and authorization are required?
- Is age entered directly or derived from another value?
- Can CID be entered or managed before the planned public API integration?
- What values are allowed for degree of kinship?
- What cardinality rules apply to patient/responsible-person/contact relationships?
- Are telephone and email both required, and can either have multiple values?
- Must patient, responsible-person, and contact records be viewable, editable, searchable, deletable, or archivable in this stage?
- How should duplicate records and incomplete registrations be handled?
- What consent, privacy, retention, and deletion rules apply to these records?
- Which public API will provide CID, and what are its availability, terms, and integration requirements?
- Which basic-data fields are required for professionals? Is CPF mandatory, and what format/checksum validation applies?
- Are certifications optional, and can a professional have multiple certifications?
- Are telephone and email mandatory for professionals, and can either have multiple values?
- Which fields can be changed when modifying a professional? How are certifications and contacts added, changed, or removed?
- What does inactivation change, and can an inactive professional be reactivated?
- Can professional associations be added to or removed from a patient after registration?
- What happens to existing patient associations when a professional is inactivated?
- Which roles may view, create, or edit anamnesis records, and what access, privacy, retention, and audit requirements apply?
- What detailed prompts or value sets belong in each anamnesis section, and what metadata is retained for each revision?
- Does professional deletion remove data permanently or use logical deletion?
- Which public API will provide professional council-registration information, and what data and integration behavior will it support?
- Which user roles may view, create, or maintain attendance and evolution records, and what privacy, retention, and audit requirements apply?
- If a professional association is removed from a patient, what happens to that professional's existing attendance records and evolution entries?
- Can an attendance or an evolution entry be deleted or cancelled, and under what conditions?
- What metadata must be retained for each evolution revision?
- Which scheduling use cases and lifecycle rules will support the future mandatory attendance-to-appointment link?
