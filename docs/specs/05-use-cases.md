# Initial-Stage Use Cases

**Status:** Draft
**Last reviewed:** 2026-10-31

## Scope

The product is a system for therapy sessions, initially focused on speech-language therapy. The confirmed product scope covers a reusable local CID-10 reference catalog, registration of patients, their responsible persons and contact methods, patient anamnesis, scheduling and maintaining patient attendances, professionals initially limited to fonoaudiólogos, and system-user account management.

The first implementation delivery is limited to the application foundation, local username/password authentication, provisioning the initial Administrator through external configuration, and system-user account management using the three fixed roles. Remaining patient, responsible-person, professional, anamnesis, attendance, and evolution capabilities stay in the implementation backlog; this sequencing does not remove them from the confirmed product scope.

The confirmed relationships and patient information are:

```text
Patient > Responsible person(s) > Contact method(s)
Patient > zero or more active, registered professionals
Professional > zero or more patients
Patient > dated anamnesis records > preserved revisions
Patient > scheduled attendances > one or more participating professionals
Attendance > one current evolution entry per participating professional
```

The patient/responsible-person hierarchy and the attributes listed in the use cases are confirmed. The local CID-10 catalog is read-only, reusable, and searchable by code or description; its structure includes chapters, groups, categories, and subcategories. Imported file sets are identified by content fingerprint when no upstream version is available. The supplied CID-O files are outside scope. The requester identifies the CID-10 files as SUS data but their upstream edition/version and terms have not been independently verified; the files and imported data are for internal use only and must not be redistributed. Each environment supplies the files locally for import. During patient registration, the user may associate zero or more already registered and active professionals with the patient; professionals may also be added later through the patient record. Each professional may be associated with multiple patients. Patient creation rejects a duplicate normalized full name and date of birth as defined in [ADR-028](./decisions/028-patient-duplicate-detection.md). A patient may be registered without an anamnesis. Anamneses are separate, dated records; creating a new record does not overwrite previous records, and editing a record preserves its earlier versions. Each version records its date, author, and timestamp; automatic deletion is not allowed pending a retention policy. An attendance concerns one patient and one or more professionals who are active and associated with the patient by the time scheduling completes. If the patient has no active associated professional, the scheduling flow offers all active professionals; selecting one or more creates the patient-professional association(s) and completes scheduling. Administrators and Attendants may schedule or reschedule. Removing an association does not delete or alter existing attendance or evolution records, and an already scheduled attendance and its participants remain unchanged. Professional inactivation preserves patient associations but prevents new scheduling involving the inactive professional; reactivation restores the preserved associations automatically. Inactivation does not alter existing attendance participation or prevent the professional from participating and recording/updating their evolution under the normal attendance-status rules. Every attendance has a scheduled date/time, a duration selected from the fixed options of 10, 20, 30, 40, 50, or 60 minutes, and actual start/end date/time recorded during realization. Attendance and evolution records are retained permanently. A rescheduled replacement requires a new duration selection. Each participating professional has a separate, optional evolution entry; updates replace its current content, and prior versions are not retained. UC-008 defines the attendance statuses, cancellation and rescheduling, participant maintenance, and late indication. Role-specific access is defined below; clinical implementation remains blocked pending privacy/legal requirements. Account username, password, session, and failed-login policies are defined in UC-009. Professional editable fields, nonclinical data retention, and patient-record update/archival behavior remain open. Professional council-registration information will be retrieved from a public API in a later stage. Do not infer additional product features such as billing.

## Actors

- **Administrador (Administrator):** Has access to all specified system operations, including system-user account management, professional records, and cancellation-reason maintenance. May cancel any `Agendado` attendance, including one they do not participate in, but may start or end an attendance only when participating. Evolution authorship remains restricted to each participating professional's own entry.
- **Atendente (Attendant):** Maintains patient records and patient-professional associations, responsible persons and their contacts, and attendance scheduling/rescheduling. May add or remove attendance participants under the UC-008 constraints, but may not cancel an attendance. The role has no access to anamnesis or evolution content.
- **Terapeuta (Therapist):** May access patient clinical records (anamneses, attendances, and evolutions) for all patients. May maintain anamneses. As a participating professional, may start, end, or cancel an attendance when allowed by its status, and may record or update only their own evolution entry. May not schedule attendances or maintain attendance participants.
- **System-user account:** Has exactly one fixed role: `Administrador`, `Atendente`, or `Terapeuta`. The account has a username, display name, and email attribute; email is not the authentication identifier. Authentication uses a local username that uniquely identifies the account and a password. Account-field requiredness and validation remain open; authentication policies are specified in UC-009.

## Use cases

### UC-001 — Register a patient and record anamnesis

**Status:** Confirmed; details open
**Goal:** Create a patient record with full name and date of birth, a CID attribute selected from the local CID-10 catalog, optionally associate one or more already registered, active professionals who will attend the patient, and optionally record dated anamnesis information.

**Main flow:**

1. An Attendant or Administrator starts patient registration.
2. The user provides the patient's full name and required date of birth.
3. The user may select zero or more already registered, active professionals to associate with the patient.
4. The Administrator may add an anamnesis, including its date of completion and information in the confirmed anamnesis sections. An Administrator or Therapist may also maintain anamneses through the patient record.
5. The system creates the patient record, records the selected professional associations, and stores the anamnesis as a dated record when provided.

**Acceptance criteria:**

- A patient record can be created.
- Full name and date of birth are patient attributes. The date of birth is required, cannot be in the future, and has no maximum age limit.
- The system derives age from the date of birth and displays months for ages below one year, then completed years.
- Patient creation is rejected when another patient has the same date of birth and a full name matching after leading/trailing whitespace is trimmed, internal whitespace is collapsed, and comparison ignores letter case while preserving accents and punctuation.
- Administrators and Attendants may create and maintain patient records and patient-professional associations.
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
- Anamnesis sections use free-text fields. Each preserved version records its date, author, and timestamp. Automatic deletion is not permitted until a retention policy is approved. Clinical anamnesis implementation remains blocked pending privacy/legal review.
- CID is a patient attribute sourced from the local catalog in [ADR-026](./decisions/026-local-cid10-reference-catalog.md). The catalog is searchable by code or description and contains the CID-10 hierarchy of chapters, groups, categories, and subcategories.
- Patient record update and archival behavior remains to be defined.

### UC-002 — Register and associate a responsible person

**Status:** Confirmed; details open
**Goal:** Record a responsible person's full name and degree of kinship, and associate that person with a patient.

**Precondition:** The patient exists or is being registered in the same flow.

**Main flow:**

1. An Attendant or Administrator selects or identifies the patient.
2. The user provides the responsible person's full name and degree of kinship.
3. The system records the responsible person associated with that patient.

**Acceptance criteria:**

- A responsible-person record can be associated with its patient.
- A patient may have multiple responsible-person records; each responsible-person record belongs to exactly one patient.
- A responsible person may be created while registering or editing the patient.
- The confirmed responsible-person attributes are full name and degree of kinship.
- Degree of kinship is selected from the closed list `Mãe`, `Pai`, `Avó/Avô`, `Irmã/Irmão`, `Tia/Tio`, `Padrasto/Madrasta`, `Cônjuge/Companheiro(a)`, `Tutor/Guardião legal`, and `Outro`. `Outro` has no free-text detail.
- Duplicate responsible-person names for the same patient are rejected after trimming leading and trailing whitespace, collapsing internal whitespace, and comparing without case sensitivity while preserving accents and punctuation.

### UC-003 — Register contact methods for a responsible person

**Status:** Confirmed; details open
**Goal:** Record telephone and email contact details associated with a responsible person.

**Precondition:** The responsible person exists or is being registered in the same flow.

**Main flow:**

1. An Attendant or Administrator selects or identifies the responsible person.
2. The user provides the person's telephone and email contact details.
3. The system records the contact information associated with that responsible person.

**Acceptance criteria:**

- Contact information can be associated with the relevant responsible-person record.
- The confirmed contact types are telephone and email.
- At least one telephone or email is required for each responsible person. Multiple values of either type are allowed; no preferred contact is defined.
- A telephone number has a Brazilian DDD and subscriber number (10 or 11 digits); the `+55` country prefix is optional. Spaces, parentheses, and hyphens may be used as separators.
- Email addresses must pass syntax validation.
- Contact values are trimmed at their boundaries. Ownership is not verified, and contact values are not otherwise normalized.

### UC-004 — Register a professional

**Status:** Confirmed; details open
**Goal:** Register a professional, initially limited to fonoaudiólogos, with basic data, CPF, certifications, and contact information.

**Main flow:**

1. The Administrator starts professional registration.
2. The user enters the professional's basic data, including CPF.
3. The user records zero or more certifications, each with a description, alphanumeric registration, and completion date.
4. The user records one or more telephone or email contacts; multiple values of either type are allowed, and at least one contact is required.
5. The system checks that the CPF does not belong to another professional.
6. The system creates the professional record if the CPF is not duplicated.

**Acceptance criteria:**

- The initial supported professional type is fonoaudiólogo. Support for other professional types is not part of the current scope.
- The professional's required basic fields are full name and CPF. CPF is required, must be valid, and defines professional uniqueness; a duplicate is rejected.
- A certification has a description, an alphanumeric registration, and a completion date.
- Professional contact types include telephone and email.
- Certifications are optional and may have multiple entries. At least one telephone or email is required; multiple values of either type are allowed, and contacts must pass format validation.
- Which professional fields can be changed and how certifications and contacts are individually maintained remain to be defined.
- Professional-council registration data is planned to be retrieved from a public API in a later stage. No API is selected, and that integration is not part of this use case's current implementation.

### UC-005 — Modify a professional

**Status:** Confirmed; details open
**Goal:** Change an existing professional record.

**Main flow:**

1. The Administrator identifies an existing professional.
2. The user changes the professional's data.
3. If the CPF is changed, the system ensures it does not duplicate another professional's CPF.
4. The system saves the changes if the CPF is not duplicated.

**Acceptance criteria:**

- An existing professional record can be modified.
- A CPF already assigned to a different professional cannot be saved.
- Which fields can be changed and how certifications and contacts are individually maintained remain to be defined.

### UC-006 — Inactivate a professional

**Status:** Confirmed; details open
**Goal:** Inactivate an existing professional record.

**Main flow:**

1. The Administrator identifies an existing professional.
2. The user requests inactivation.
3. The system marks the professional as inactive.

**Acceptance criteria:**

- A professional can be inactivated.
- Existing patient associations are retained during professional inactivation. The inactive professional cannot be used for new scheduling; reactivation restores the retained associations automatically.

### UC-007 — Delete a professional

**Status:** Confirmed; details open
**Goal:** Remove a professional record when no patient is associated with that professional.

**Main flow:**

1. The Administrator identifies a professional and requests deletion.
2. The system checks whether any patient is associated with the professional.
3. If a patient is associated, the system prevents deletion.
4. If no patient is associated, the system permits deletion.

**Acceptance criteria:**

- A professional associated with one or more patients cannot be deleted.
- Deletion is permitted only when no patient is associated with the professional.
- Deletion is logical, terminal, and distinct from reversible inactivation. Historical references are preserved, and a deleted professional cannot be reactivated.

### UC-008 — Record and maintain a patient attendance

**Status:** Confirmed; details open
**Goal:** Schedule and maintain a patient attendance with one or more associated professionals, record its realization, and maintain each participating professional's optional evolution.

**Preconditions:**

- The patient exists.
- For scheduling or rescheduling, the actor is an Administrator or Attendant.

**Main flow:**

1. An Administrator or Attendant starts or reschedules an attendance for the patient. The user does not have to participate in the attendance.
2. The system offers active, registered professionals already associated with the patient. If no active professional is associated, the system offers all active, registered professionals. The user selects one or more professionals; any selected professional not yet associated is linked to the patient as part of scheduling.
3. The actor provides the required scheduled date and time and selects a duration from the fixed options of 10, 20, 30, 40, 50, or 60 minutes. If the attendance is recorded without an existing schedule, the system creates its scheduling period at that time.
4. The system creates the attendance in the `Agendado` status and prevents overlapping with other `Agendado` attendances for any selected professional. Attendances in other statuses do not count as scheduling conflicts.
5. Any participating professional may start the attendance before or after its scheduled date/time. The system changes its status to `Em andamento` and records the actual start date/time.
6. After the attendance is saved, an Attendant or Administrator may add additional active, registered professionals already associated with the patient while the attendance is in any status except `Cancelado`.
7. Any participating professional may end the attendance. The system records the actual end date/time and sets its status to `Finalizado` if at least one evolution exists, or `Aguardando Evolução` if none exists.
8. While the attendance is `Em andamento` or `Aguardando Evolução`, each participating professional may record or update only their own evolution entry. It has separate fields for the therapeutic plan carried out, which is required, and the professional's observations during the attendance, which are optional. Updating replaces the current entry; prior versions are not retained.
9. If an evolution is added after the attendance has entered `Aguardando Evolução`, the system automatically changes its status to `Finalizado`. A participating professional without an evolution remains individually indicated as pending, even when the attendance is `Finalizado`.
10. While the attendance is `Agendado`, an Attendant or Administrator may remove a participating professional only if at least one other participant remains and the professional has no evolution recorded.
11. While the attendance is still `Agendado`, any participating professional or an Administrator may cancel it by selecting a required reason from the cancellation-reason domain. Free-text detail is optional. The patient nonattendance reason may be selected at any time while the attendance is `Agendado`, including before its scheduled date/time.
12. If a scheduled attendance needs a new date/time, the system automatically cancels the existing attendance and creates a replacement linked to the cancelled record. It keeps existing active participants and removes any inactive participants. If no active participant remains, the system offers active professionals already associated with the patient; if none are associated, it offers all active professionals and creates a patient-professional association for the selection. The user selects a new duration for the replacement from the fixed options of 10, 20, 30, 40, 50, or 60 minutes. The system assigns the fixed `Remarcação` reason automatically without requiring the user to cancel and recreate the attendance manually.
13. If the scheduled date/time passes before the attendance starts, its status remains `Agendado`; the system indicates that it is late by comparing the scheduled date/time with the current date/time. This is an on-screen indication, not a status. The professional may still start the attendance.

**Acceptance criteria:**

- The patient nonattendance reason may be selected at any time while the attendance is `Agendado`, including before its scheduled date/time.
- Every attendance has a scheduled date/time, concerns exactly one patient, and has one or more participating professionals.
- An Administrator or Attendant may schedule or reschedule the attendance; the user does not have to participate.
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
- Each participating professional may record or update only their own evolution while the attendance is `Em andamento` or `Aguardando Evolução`. Evolution entries cannot be recorded or updated while the attendance is `Agendado`, `Finalizado`, or `Cancelado`; this authorship restriction also applies to Administrators.
- When an evolution is added after status becomes `Aguardando Evolução`, the system changes the status to `Finalizado` automatically. An individual participant with no evolution remains indicated as pending even when the attendance status is `Finalizado`.
- Updating an evolution replaces its current content; previous versions are not preserved.
- An evolution entry cannot be deleted.
- The defined maintenance operations are adding or removing participating professionals and recording or updating evolution entries. No other attendance data may be edited.
- An Attendant or Administrator may add additional active, registered professionals already associated with the patient after the attendance is saved while its status is `Agendado`, `Em andamento`, `Aguardando Evolução`, or `Finalizado`. They may not be added when status is `Cancelado`.
- An Attendant or Administrator may remove a participating professional only while status is `Agendado`, only if at least one other participant remains, and only if the professional has no evolution recorded. Removal is not allowed in other statuses.
- Only an attendance in `Agendado` may be cancelled; any participating professional or an Administrator may perform the cancellation. A cancellation requires selecting a reason from the cancellation-reason domain; free-text detail is optional. An Administrator maintains the configurable reasons `Não comparecimento`, `Indisponibilidade do profissional`, and `Solicitação do paciente/responsável` through create, list, edit, and inactivate operations. These three reasons are created as active records when the feature is first installed. Each configurable reason has a required name, optional description, and active/inactive indicator; names are unique across active and inactive records after trimming leading and trailing whitespace and comparing without case sensitivity while preserving accents and punctuation. Configurable reasons cannot be deleted. An inactivated reason is unavailable for new cancellations but remains visible on historical attendances that used it. Editing a reason does not change the original name and description shown in historical attendances. `Remarcação` is a fixed system reason, is assigned automatically during rescheduling, and is not maintained through that feature.
- An Administrator may start or end an attendance only when participating; the Administrator's non-participant override applies only to cancellation.
- Rescheduling automatically cancels the existing `Agendado` attendance and creates a replacement linked to the previous record. Existing active participants remain; inactive participants are removed. If none remain, the system offers active professionals already associated with the patient; if none are associated, it offers all active professionals and creates a patient-professional association for the selection. The replacement uses the requested new date/time and a new duration, and the system assigns the fixed `Remarcação` reason automatically.
- If the scheduled date/time passes before start, the attendance remains `Agendado` and the system displays a late indication based on comparison with current date/time. It is not a separate status, and a professional may still start the attendance.
- Attendance records and evolution entries are retained permanently and cannot be deleted, including records with status `Cancelado`. Role-based access is specified under Actors; other privacy, legal, and data-handling requirements remain to be defined.
- Implementation of anamnesis, attendance, and evolution features remains blocked until privacy/legal review defines the applicable consent/legal basis, data-handling requirements, and retention policy.

### UC-009 — Manage system-user accounts and roles

**Status:** Confirmed; details open
**Goal:** Manage system-user accounts and assign exactly one initial role to each account.

**Preconditions:**

- An Administrator is authenticated. The initial Administrator is provisioned from configuration supplied outside the source code and must change the configured password at first login.

**Main flow:**

1. The Administrator creates or edits a system-user account.
2. The Administrator assigns or changes exactly one role: `Administrador`, `Atendente`, or `Terapeuta`.
3. When creating an account, the system generates a temporary password, displays it once, and requires the new user to change it at first login.
4. The Administrator may activate or inactivate the account. Password reset/recovery is not part of the first implementation delivery.

**Acceptance criteria:**

- Only an Administrator may create or edit system-user accounts, assign or change roles, activate or inactivate accounts, or initiate an access reset.
- Each account has exactly one of the three defined roles: `Administrador`, `Atendente`, or `Terapeuta`.
- A system-user account must have a display name. The system trims leading and trailing whitespace and rejects a display name that is empty after trimming; no other display-name format or length rules apply.
- Email is optional. When provided, the system trims leading and trailing whitespace and validates a conventional email-address format. Email uniqueness and ownership verification are not required.
- An inactivated account cannot authenticate.
- Authentication is local and uses username and password; email is an account attribute and is not used to authenticate.
- The initial Administrator is provisioned through configuration supplied outside the source code and must change the configured password at first login.
- Bootstrap creates the initial active `Administrador` account with a required first-login password change only when neither the normalized configured username nor an Administrator account already exists. If the configured username matches an Administrator account, startup continues without changing that account, even if its password, active state, or password-change requirement differs from configuration. If the configured username matches a non-Administrator account, or an Administrator exists under a different username, startup fails without modifying or creating an account.
- For an account created by an Administrator, a system-generated temporary password is displayed only once and must be changed at first login.
- Usernames are normalized by trimming leading and trailing whitespace and comparing case-insensitively for uniqueness and authentication.
- A password set by a user must contain at least 12 characters; no character-composition rules are required. The same password policy applies when changing the initial Administrator password and a temporary password.
- An authenticated session expires after 30 minutes of inactivity.
- Five consecutive failed login attempts for the same normalized username cause a 15-minute lockout. The failure counter resets when the lockout expires or after a successful authentication. Login failure messages do not reveal whether the username exists.
- A successful authentication resets the consecutive failed-login attempt count.
- The three roles are fixed for the first implementation delivery; creating or editing role definitions and permissions is not included.
- Password reset/recovery is deferred from the first implementation delivery.
- Account deletion is not specified in the initial scope.
- Access-reset mechanics and account deletion remain open.

## Open questions

- Should username casing be preserved when displayed, or should a canonical case be used?
- What access-reset/recovery mechanics and account-deletion behavior should be specified for a later delivery?
- What lifecycle operations are required for patient, responsible-person, and contact records?
- What consent, privacy, retention, and deletion rules apply to these records?
- Which specific validation and normalization rules apply to CPF, telephone, and email?
- Which fields can be changed when modifying a professional? How are certifications and contacts added, changed, or removed?
- What privacy, legal, retention, and data-handling requirements apply to anamnesis beyond the defined role-based access?
- Which public API will provide professional council-registration information, and what data and integration behavior will it support?
- What privacy, legal, and data-handling requirements apply to attendance and evolution records beyond the defined role-based access?
