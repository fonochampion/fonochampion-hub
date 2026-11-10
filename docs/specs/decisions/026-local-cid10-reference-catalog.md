# ADR-026: Local CID-10 Reference Catalog

**Status:** Accepted
**Date:** 2026-11-10

## Context

The patient record includes a CID attribute, but the previously planned public API is not the chosen source for the catalog. The product needs a reusable local Brazilian Portuguese CID-10 catalog that can be queried by code or description. The requester supplied separate CID-10 and CID-O files; only CID-10 is in scope. The requester identifies the CID-10 files as SUS data, reports that the upstream version is unknown, and restricts use to internal environments without redistribution.

## Options considered

1. Retrieve CID data at runtime from a public API.
2. Maintain a local, read-only catalog loaded from reference files supplied separately in each environment.

## Decision

- Use a full Brazilian Portuguese CID-10 catalog as local reference data; do not use a runtime public CID API for this scope.
- Make the catalog read-only and reusable, with lookup by code or description.
- Import its hierarchy of chapters, groups, categories, and subcategories.
- Exclude the supplied CID-O files.
- Do not commit, package, or redistribute the source files or imported catalog data. Identify each imported file set by content fingerprint when no upstream version is available; do not represent that fingerprint as an official SUS edition.
- Patient-record integration remains separate backlog work under IMP-005 and depends on the catalog being implemented under IMP-026. Patient date-of-birth and age-display behavior is defined in ADR-027.

## Consequences

- **Benefits:** CID lookup does not depend on runtime API availability or transmit lookup terms to an external provider; the catalog can be reused by application features.
- **Costs or risks:** Each environment must receive the reference files separately. The upstream edition and usage terms are unverified, and the dataset must not be redistributed.
- **Follow-up actions:** Implement the local import and lookup under IMP-026; implement patient integration separately under IMP-005.
