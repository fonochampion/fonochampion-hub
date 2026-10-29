# ADR-007: Initial Professional Registration Scope

**Status:** Accepted
**Date:** 2026-10-29

## Context

The requester defined professional registration as part of the initial system scope, with specific fields and maintenance rules. Some data and lifecycle details remain unresolved.

## Options considered

1. Include professional registration and maintenance in the initial scope with the confirmed rules.
2. Leave professional registration outside the initial scope.

## Decision

- Initially register only fonoaudiólogos; support for other professions may be considered in the future but is not included now.
- Professional records include basic data and CPF. CPF defines uniqueness among professionals; CPF requiredness and validation remain open.
- Certifications contain a description, an alphanumeric registration, and a completion date. Requiredness and multiplicity remain open.
- Professional contacts include telephone and email. Requiredness, multiplicity, and validation remain open.
- Support creating, modifying, inactivating, and deleting a professional.
- Do not delete a professional while any patient is associated with that professional. See [ADR-008](./008-patient-professional-association.md) for the confirmed relationship rules.
- Retrieve professional council-registration information through a public API in a later stage. The API provider, data, and behavior are not selected, and the integration is deferred.

Detailed flows and acceptance criteria are in [05 — Use Cases](../05-use-cases.md).

## Consequences

- **Benefits:** Defines the initial professional-registration scope while preserving unresolved field and lifecycle details.
- **Costs or risks:** The professional-council API cannot be implemented until its provider and requirements are selected. Some professional lifecycle behavior remains open.
- **Follow-up actions:** Resolve open professional validation and lifecycle questions before implementing dependent behavior; see [ADR-008](./008-patient-professional-association.md) for the confirmed patient association.
