---
name: implementation-planning
description: "Use to assess implementation readiness, prioritize the implementation roadmap, or execute one explicitly requested roadmap item."
---

# Implementation Planning and Execution

Use confirmed specifications as the source of truth and `docs/specs/06-implementation-checklist.md` as the implementation roadmap. Run planning only after the approved feature or change is documented. Keep planning separate from execution: never implement an item before the requester selects its checklist ID(s).

## Planning workflow

1. Read `AGENTS.md`, the roadmap, the specification index, and the relevant specifications and accepted decisions.
2. Review all unfinished roadmap items against their confirmed scope, acceptance criteria, dependencies, and decision gates. Do not infer behavior or close an open gate.
3. An item is ready only when its scope is sufficiently specified, all listed dependencies are `DONE`, and all relevant decision gates are resolved. Prioritize ready items with no dependencies first; then list other ready items in dependency order.
4. Update the roadmap to reflect readiness and priority without changing item IDs, expanding scope, or marking work `IN PROGRESS` or `DONE`. Preserve completed history. Add implementation items only for requester-approved scope.
5. Report ready items, emphasizing those with no dependencies, and list blocked items with the specific dependency or open decision. Stop after planning; wait for the requester to select one or more ready item IDs. Selection authorizes only those IDs, not adjacent or dependent work that was not selected.

Readiness is not permission to implement. Follow all repository and session instructions. In particular, while `AGENTS.md` restricts work to agent artifacts and specifications during setup, do not create or modify application code, even when a roadmap item is otherwise ready.

## Selected-item execution workflow

When the requester explicitly selects one or more roadmap item IDs for implementation:

1. Confirm the selected IDs and re-read each item's requirements, decisions, dependencies, and acceptance criteria.
2. If any selected item is blocked, has an unresolved decision, or depends on unfinished work outside the selected set, explain the blocker and do not start that item. Do not make the decision or bypass the dependency yourself.
3. If repository instructions prohibit implementation, explain the blocker and stop without changing code or item statuses.
4. Set only the selected, unblocked items to `IN PROGRESS` before implementation. Implement only their confirmed scope, in dependency order where applicable; do not start unselected items.
5. Ask for clarification when an unresolved choice could materially affect scope, UX, data, security, or cost. Run the smallest relevant tests and checks for each selected item. Mark an item `DONE` only after its relevant validation passes; otherwise leave it `IN PROGRESS` and report the failure.
6. Report the selected IDs, changes, validation, and remaining blockers. Do not automatically proceed to unselected items.

## Boundaries

- Never treat technology choices or implementation convenience as confirmation of unspecified product behavior.
- Do not add deferred or unapproved features to the roadmap.
- Do not mark an item done based only on code being written; its relevant validation must pass.
- Keep roadmap statuses and dependencies accurate. When a new approved item changes dependencies, update the affected roadmap entries consistently.
