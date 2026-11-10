---
name: implementation-planning
description: "Use to report remaining work, assess roadmap readiness, and prioritize approved implementation items. Planning only; never implement."
---

# Implementation Planning

Use confirmed specifications as the source of truth and `docs/specs/06-implementation-checklist.md` as the roadmap. This skill is for planning and status reporting only.

## Workflow

1. Read `AGENTS.md`, the roadmap, the specification index, and only the relevant specifications and accepted decisions.
2. Review unfinished items against their confirmed scope, acceptance criteria, dependencies, and decision gates. Do not infer behavior or close an open gate.
3. An item is ready only when its scope is sufficiently specified, all listed dependencies are `DONE`, and all relevant decision gates are resolved.
4. When asked what remains, report `PENDING`, `BLOCKED`, and `IN PROGRESS` items in roadmap order; exclude `DONE` items and explain blockers.
5. Update readiness and priority only when requested. Preserve item IDs and completed history; do not change status to `IN PROGRESS` or `DONE` during planning.
6. Report ready items, prioritizing those with no dependencies, then other ready items in dependency order. Stop after planning and wait for item selection.

## Boundaries

- Readiness is not implementation authorization.
- Never implement code or change an item's status to `IN PROGRESS` or `DONE` from this skill.
- Use `.agents/skills/implement-selected-items/SKILL.md` for execution of explicitly selected item IDs.
- Never add deferred or unapproved features to the roadmap.
