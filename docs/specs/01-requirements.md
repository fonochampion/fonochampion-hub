# Requirements

**Status:** Draft
**Last reviewed:** 2026-10-29

## Functional requirements

Record each confirmed requirement with a stable identifier and observable acceptance criteria.

| ID | Requisito | Critérios de aceite | Status |
|---|---|---|---|
| FR-001 | Register a patient with full name and age. | A system user can create a patient record with full name and age. Age representation, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-002 | Associate a list of responsible persons with a patient. | A patient record can be associated with responsible persons; each responsible person has a full name and a degree of kinship. The kinship domain, cardinality constraints, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-003 | Register telephone and email contact details for a responsible person. | A responsible person can have telephone and email contact details. Requiredness, multiplicity, validation, and user permissions remain to be defined. | Confirmed; details open |
| FR-004 | Record the patient's CID (International Classification of Diseases). | CID is a patient attribute. Retrieval from a public API is planned for a later stage; the API, integration behavior, and whether CID is available before that stage remain to be defined. | Confirmed; integration deferred |

The initial functional scope is limited to the patient > responsible persons > contact methods registration hierarchy described in [05 — Use Cases](./05-use-cases.md). Do not infer appointment scheduling, therapy-session records, clinical notes, billing, or other features from the product description.

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
- User roles, authentication, and authorization remain open.
- Data classification, consent/legal basis, access controls, retention, and deletion requirements remain open; see [04 — Data and Integrations](./04-data-and-integrations.md).
