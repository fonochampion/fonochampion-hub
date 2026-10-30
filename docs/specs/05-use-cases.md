# Initial-Stage Use Cases

**Status:** Draft
**Last reviewed:** 2026-10-30

## Scope

The product is a system for therapy sessions, initially focused on speech-language therapy. The confirmed initial stage covers registration of patients, their responsible persons and contact methods, patient anamnesis, scheduling and maintaining patient attendances, and professionals initially limited to fonoaudiólogos.

The confirmed relationships and patient information are:

```text
Patient > Responsible person(s) > Contact method(s)
Patient > zero or more active, registered professionals
Professional > zero or more patients
Patient > dated anamnesis records > preserved revisions
Patient > scheduled attendances > one or more participating professionals
Attendance > one current evolution entry per participating professional
```

The patient/responsible-person hierarchy and the attributes listed in the use cases are confirmed. During patient registration, the system user may associate zero or more already registered and active professionals with the patient; professionals may also be added later through the patient record. Each professional may be associated with multiple patients. A patient may be registered without an anamnesis. Anamneses are separate, dated records; creating a new record does not overwrite previous records, and editing a record preserves its earlier versions. An attendance concerns one patient and one or more professionals who are active and associated with the patient by the time scheduling completes. If the patient has no active associated professional, the scheduling flow offers all active professionals; selecting one or more creates the patient-professional association(s) and completes scheduling. Scheduling may be initiated by any system user with scheduling permission; the authorized roles remain to be defined. Removing an association does not delete or alter existing attendance or evolution records, and an already scheduled attendance and its participants remain unchanged. Professional inactivation preserves patient associations but prevents new scheduling involving the inactive professional; reactivation restores the preserved associations automatically. Inactivation does not alter existing attendance participation or prevent the professional from participating and recording/updating their evolution under the normal attendance-status rules. Every attendance has a scheduled date/time, a duration selected from the fixed options of 10, 20, 30, 40, 50, or 60 minutes, and actual start/end date/time recorded during realization. Attendance and evolution records are retained permanently. A rescheduled replacement requires a new duration selection. Each participating professional has a separate, optional evolution entry; updates replace its current content, and prior versions are not retained. UC-008 defines the attendance statuses, cancellation and rescheduling, participant maintenance, and late indication. Age representation, CID availability before its planned API integration, kinship values, responsible-person/contact cardinality, requiredness, validation, clinical-data access controls, retention for other data, cancellation-reason configuration, and patient-record lifecycle remain open. Professional council-registration information will be retrieved from a public API in a later stage. Do not infer additional product features such as billing.

## Actors

- **System user:** Performs registration. Specific user roles, authentication, and permissions remain to be defined.
- **Attending professional:** Participates in an attendance, may start and end it, may cancel it while scheduled, and may record or update only their own evolution entry. Access-control rules for other actions remain to be defined.

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
- A professional association may be removed from a patient. Removal does not delete or alter existing attendance or evolution records; already scheduled attendances and their participants remain unchanged.
- Inactivating a professional preserves existing patient associations. While inactive, the professional cannot be used for new scheduling; reactivation restores the preserved associations automatically.
- Inactivation does not alter an existing attendance or the professional's participation. The professional may continue participating and record/update their own evolution when allowed by the attendance status.
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
- Existing patient associations are retained during professional inactivation. The inactive professional cannot be used for new scheduling; reactivation restores the retained associations automatically.

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
**Goal:** Schedule and maintain a patient attendance with one or more associated professionals, record its realization, and maintain each participating professional's optional evolution.

**Preconditions:**

- The patient exists.
- For scheduling or rescheduling, the actor has scheduling permission; which roles have this permission remains to be defined.

**Main flow:**

1. A system user with scheduling permission starts or reschedules an attendance for the patient. The user does not have to participate in the attendance.
2. The system offers active, registered professionals already associated with the patient. If no active professional is associated, the system offers all active, registered professionals. The user selects one or more professionals; any selected professional not yet associated is linked to the patient as part of scheduling.
3. The actor provides the required scheduled date and time and selects a duration from the fixed options of 10, 20, 30, 40, 50, or 60 minutes. If the attendance is recorded without an existing schedule, the system creates its scheduling period at that time.
4. The system creates the attendance in the `Agendado` status and prevents overlapping with other `Agendado` attendances for any selected professional. Attendances in other statuses do not count as scheduling conflicts.
5. Any participating professional may start the attendance before or after its scheduled date/time. The system changes its status to `Em andamento` and records the actual start date/time.
6. After the attendance is saved, additional active, registered professionals already associated with the patient may be added while the attendance is in any status except `Cancelado`.
7. Any participating professional may end the attendance. The system records the actual end date/time and sets its status to `Finalizado` if at least one evolution exists, or `Aguardando Evolução` if none exists.
8. While the attendance is `Em andamento` or `Aguardando Evolução`, each participating professional may record or update only their own evolution entry. It has separate fields for the therapeutic plan carried out, which is required, and the professional's observations during the attendance, which are optional. Updating replaces the current entry; prior versions are not retained.
9. If an evolution is added after the attendance has entered `Aguardando Evolução`, the system automatically changes its status to `Finalizado`. A participating professional without an evolution remains individually indicated as pending, even when the attendance is `Finalizado`.
10. While the attendance is `Agendado`, a participating professional may be removed only if at least one other participant remains and the professional has no evolution recorded.
11. While the attendance is still `Agendado`, any participating professional may cancel it by selecting a required reason from the cancellation-reason domain. Free-text detail is optional. The patient nonattendance reason may be selected at any time while the attendance is `Agendado`, including before its scheduled date/time.
12. If a scheduled attendance needs a new date/time, the system automatically cancels the existing attendance and creates a replacement linked to the cancelled record. It keeps existing active participants and removes any inactive participants. If no active participant remains, the system offers active professionals already associated with the patient; if none are associated, it offers all active professionals and creates a patient-professional association for the selection. The user selects a new duration for the replacement from the fixed options of 10, 20, 30, 40, 50, or 60 minutes. The system assigns the fixed `Remarcação` reason automatically without requiring the user to cancel and recreate the attendance manually.
13. If the scheduled date/time passes before the attendance starts, its status remains `Agendado`; the system indicates that it is late by comparing the scheduled date/time with the current date/time. This is an on-screen indication, not a status. The professional may still start the attendance.

**Acceptance criteria:**

- The patient nonattendance reason may be selected at any time while the attendance is `Agendado`, including before its scheduled date/time.
- Every attendance has a scheduled date/time, concerns exactly one patient, and has one or more participating professionals.
- A system user with scheduling permission may schedule or reschedule the attendance; the user does not have to participate. Which roles have this permission remains to be defined.
- Each attendance has a duration selected from the fixed options of 10, 20, 30, 40, 50, or 60 minutes. The system uses the scheduled date/time and duration to prevent overlapping scheduled attendances for patients and professionals. A replacement created by rescheduling requires a new duration selection.
- If an attendance is recorded without an existing schedule, the system creates its scheduling period at that time and requires its date/time.
- Removing a patient-professional association does not delete or alter existing attendance/evolution records. An already scheduled attendance and its participants remain unchanged.
- Inactivation does not alter existing attendance participation or prevent the professional from participating and recording/updating their own evolution under the normal attendance-status rules.
- When scheduling begins, if active professionals are associated with the patient, only those professionals are offered. If none are associated, the system offers all active, registered professionals; it associates selected professionals with the patient as part of scheduling.
- Only active, registered professionals already associated with the patient may be added as participants after the attendance is saved.
- A professional may participate in attendances for multiple patients.
- The system prevents overlapping attendances for the same professional when the existing attendance is `Agendado`; attendances in `Em andamento`, `Cancelado`, or `Finalizado` do not count as scheduling conflicts. The system also prevents overlapping attendances for the same patient.
- The attendance has separate scheduled and realization periods. The system records actual start and end date/time when a participating professional starts and ends it.
- The defined attendance statuses are `Agendado`, `Em andamento`, `Finalizado`, `Aguardando Evolução`, and `Cancelado`.
- The system preserves a history of attendance status changes, recording the author and date/time for each change. For an automatic transition, the author is the professional or user whose action triggered it.
- Creating the schedule sets status to `Agendado`. Any participating professional may start the attendance before or after the scheduled date/time, changing it to `Em andamento`.
- Any participating professional may end an attendance; status is `Finalizado` if at least one evolution exists, otherwise `Aguardando Evolução`.
- Each participating professional has an individual evolution entry; the entry is optional.
- An evolution has separate fields for the therapeutic plan carried out and the professional's observations during the attendance. The plan field is required; the observations field is optional.
- The system records the author and date/time when an evolution is created and the author and date/time of its most recent update.
- No additional audit metadata is required beyond the status-change and evolution metadata specified above.
- The system indicates when a professional's evolution has not yet been recorded. This state can later be updated when an evolution is entered.
- An evolution summarizes the therapeutic plan carried out and records observations made by the professional during the attendance.
- Each participating professional may record or update only their own evolution while the attendance is `Em andamento` or `Aguardando Evolução`. Evolution entries cannot be recorded or updated while the attendance is `Agendado`, `Finalizado`, or `Cancelado`. Permissions for other user roles remain to be defined.
- When an evolution is added after status becomes `Aguardando Evolução`, the system changes the status to `Finalizado` automatically. An individual participant with no evolution remains indicated as pending even when the attendance status is `Finalizado`.
- Updating an evolution replaces its current content; previous versions are not preserved.
- An evolution entry cannot be deleted.
- The defined maintenance operations are adding or removing participating professionals and recording or updating evolution entries. No other attendance data may be edited.
- Additional active, registered professionals already associated with the patient may be added after the attendance is saved while its status is `Agendado`, `Em andamento`, `Aguardando Evolução`, or `Finalizado`. They may not be added when status is `Cancelado`.
- A participating professional may be removed only while status is `Agendado`, only if at least one other participant remains, and only if the professional has no evolution recorded. Removal is not allowed in other statuses.
- Only an attendance in `Agendado` may be cancelled; any participating professional may perform the cancellation. A cancellation requires selecting a reason from the cancellation-reason domain; free-text detail is optional. A feature will maintain configurable reasons, including patient nonattendance, and will support creating, listing, editing, and inactivating them. Each configurable reason has a required name, optional description, and active/inactive indicator; names are unique across active and inactive reasons. Configurable reasons cannot be deleted. An inactivated reason is unavailable for new cancellations but remains visible on historical attendances that used it. Editing a reason does not change the original name and description shown in historical attendances. Permissions to maintain reasons and the complete reason values remain to be defined. `Remarcação` is a fixed system reason, is assigned automatically during rescheduling, and is not maintained through that feature.
- Rescheduling automatically cancels the existing `Agendado` attendance and creates a replacement linked to the previous record. Existing active participants remain; inactive participants are removed. If none remain, the system offers active professionals already associated with the patient; if none are associated, it offers all active professionals and creates a patient-professional association for the selection. The replacement uses the requested new date/time and a new duration, and the system assigns the fixed `Remarcação` reason automatically.
- If the scheduled date/time passes before start, the attendance remains `Agendado` and the system displays a late indication based on comparison with current date/time. It is not a separate status, and a professional may still start the attendance.
- Remaining configurable reason values, permissions to maintain reasons, and which user roles have scheduling permission remain to be defined.
- Attendance records and evolution entries are retained permanently and cannot be deleted, including records with status `Cancelado`. Attendance/evolution access controls for other roles and privacy requirements remain to be defined.

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
- Which roles may view, create, or edit anamnesis records, and what access, privacy, retention, and audit requirements apply?
- What detailed prompts or value sets belong in each anamnesis section, and what metadata is retained for each revision?
- Does professional deletion remove data permanently or use logical deletion?
- Which public API will provide professional council-registration information, and what data and integration behavior will it support?
- Which user roles may view, create, or maintain attendance and evolution records, and what privacy requirements apply?
- Who may maintain reasons, and what additional reason values are needed beyond patient nonattendance?
- Which user roles have permission to schedule or reschedule attendances?
