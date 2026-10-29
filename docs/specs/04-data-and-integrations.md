# Data and Integrations

**Status:** Draft
**Last reviewed:** 2026-10-29

## Data

- **Required entities and relationships:** Patient (full name, age, CID) > responsible person(s) (full name, degree of kinship) > contact details (telephone, email), for the initial registration stage. Age representation, CID availability before the future API integration, kinship values, cardinality, and validation rules remain to be defined. See [05 — Use Cases](./05-use-cases.md).
- **Data sources and ownership:** To be defined
- **Personal or sensitive data:** Patient and responsible-person records include names, age, CID, kinship, telephone, and email. Applicable handling requirements and classification remain to be defined.
- **Retention, deletion, and export:** To be defined

## External integrations

| Service | Purpose | Data shared | Status |
|---|---|---|---|
| Public CID API | Retrieve a patient's CID | To be defined | Planned for a later stage; provider and integration details open |

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
- Confirm whether any clinical or therapy-session data is in scope for a later stage; it is not included in the initial registration scope.
