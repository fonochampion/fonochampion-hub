# ADR-025: Agent Guidance and Skill Boundaries

**Status:** Accepted
**Date:** 2026-11-10

## Context

Agent guidance is split across root instructions, separate rule and workflow directories, and skills. Their responsibilities overlap, and skills repeat product requirements that already have a source of truth in `docs/specs/`. Some instructions also conflict with current specifications.

## Options considered

1. Keep the current files and add more framework-like directories and documents.
2. Use a concise repository entry point, give each skill one clear task boundary, and keep product requirements in the specifications.

## Decision

- Keep `AGENTS.md` as the concise repository entry point and skill selector.
- Keep each skill focused on one task, with explicit use conditions, boundaries, expected outcome, and references to only the relevant specifications.
- Keep `docs/specs/` as the product source of truth; do not duplicate its requirements in skills.
- Consolidate duplicate workflow instructions into the corresponding skill.
- Support Copilot CLI only in this repository's agent guidance.
- Use `.agents/skills/` for task-specific capabilities and `AGENTS.md` as the repository entry point.
- Remove separate rule and workflow directories that are not part of the Copilot CLI instruction and skill discovery paths.
- When repository instructions conflict, the precedence is `docs/specs/` > `AGENTS.md` > skill instructions.
- Keep skills small and specialized. A skill is valid only after it has a clear responsibility, a reviewable validation rule, and passes repository discovery/compatibility checks for Copilot CLI.
- Stale or duplicated guidance is removed only after explicit review and verification; it is not silently kept in parallel.

## Consequences

- **Benefits:** Clearer task selection, less repeated context, and lower risk of contradictory instructions.
- **Costs or risks:** The guidance is tailored to Copilot CLI and is not designed for other agent integrations.
- **Follow-up actions:** Refactor repository guidance and skills, reconcile stale instructions, and verify discovery through Copilot CLI.
