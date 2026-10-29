# Initial-Stage Use Cases

**Status:** Draft
**Last reviewed:** 2026-10-29

## Scope

The product is a system for therapy sessions, initially focused on speech-language therapy. The confirmed initial stage covers registration of patients, their responsible persons, and those persons' contact methods.

The relationship is described as:

```text
Patient > Responsible person(s) > Contact method(s)
```

The hierarchy and the attributes listed in the use cases are confirmed. Age representation, CID availability before its planned API integration, kinship values, cardinality, requiredness, validation, and record lifecycle remain open. Do not infer additional product features such as appointment scheduling, therapy-session records, clinical notes, or billing.

## Actors

- **System user:** Performs registration. Specific user roles, authentication, and permissions remain to be defined.

## Use cases

### UC-001 — Register a patient

**Status:** Confirmed; details open
**Goal:** Create a patient record with full name and age, and a CID attribute whose retrieval by a public API is planned for a later stage.

**Main flow:**

1. The system user starts patient registration.
2. The user provides the patient's full name and age.
3. The system creates the patient record.

**Acceptance criteria:**

- A patient record can be created.
- Full name and age are patient attributes.
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
- Are appointment scheduling, therapy-session records, and clinical notes explicitly planned for a later stage?
