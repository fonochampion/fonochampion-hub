# ADR-014: Evolution Entry Versioning

**Status:** Accepted
**Date:** 2026-10-30

## Context

The prior attendance specification required preserving prior versions when an evolution entry was updated. The requester changed this requirement and confirmed that version history is not needed. Each professional has one evolution entry per attendance.

## Options considered

1. Preserve every prior evolution entry version.
2. Keep only the current evolution entry and replace its content when updated.

## Decision

Each participating professional has one optional current evolution entry per attendance. Updating the entry replaces its current content; prior versions are not retained. The entry cannot be deleted. Its plan field is required and its observations field is optional. The system records the creator and creation date/time, and the author and date/time of the most recent update. The professional may record or update only their own entry while the attendance is `Em andamento` or `Aguardando Evolução`.

This decision supersedes the prior-version-preservation requirement for evolution entries in ADR-013. Anamnesis revision preservation is unchanged.

## Consequences

- **Benefits:** Evolution maintenance remains limited to one current entry per professional and attendance.
- **Costs or risks:** Earlier content cannot be reconstructed after an update.
- **Follow-up actions:** No additional evolution audit metadata is required beyond the creator/creation and last updater/update date/time specified in this decision.
