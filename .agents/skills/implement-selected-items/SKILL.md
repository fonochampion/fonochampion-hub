---
name: implement-selected-items
description: "Use only after the requester explicitly selects implementation roadmap item IDs."
---

# Implement Selected Roadmap Items

Implement only the item IDs explicitly selected by the requester from `docs/specs/06-implementation-checklist.md`.

## Workflow

1. Read `AGENTS.md`, the selected roadmap items, their dependencies, and only the relevant specifications and accepted decisions.
2. If a selected item is blocked, has an unresolved decision, or depends on unfinished work outside the selected set, explain the blocker and stop that item. Do not infer a decision or implement adjacent work.
3. Before implementation, set only the selected, unblocked items to `IN PROGRESS`.
4. Implement only their confirmed scope. Ask for clarification if an unresolved choice could change scope, user experience, data, security, architecture, or cost.
5. Run the smallest relevant tests and checks. Mark an item `DONE` only after implementation and validation pass; otherwise leave it `IN PROGRESS` and report the failure.
6. Report the selected IDs, changes, validation, and remaining blockers. Do not proceed to unselected items.

## Boundaries

- Roadmap readiness is not implementation authorization; explicit item selection is required.
- Preserve unrelated work and completed roadmap history.
- Do not introduce requirements or implementation artifacts outside the selected items.
