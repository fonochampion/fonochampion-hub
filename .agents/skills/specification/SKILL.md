---
name: specification
description: "Use to analyze a proposed product change and prepare a decision brief before documentation or implementation."
---

# Product Change Discovery

Use this skill when the requester wants to explore, analyze, or specify a product change.

## Workflow

1. Read `AGENTS.md`, the specification index, and only the relevant specifications, accepted decisions, and roadmap items.
2. Separate confirmed facts, assumptions, and open questions. Identify conflicts, dependencies, alternatives, affected documents, and likely implementation work.
3. Ask focused questions when an unresolved choice could change scope, user experience, data, security, architecture, or cost.
4. Present a decision brief in the requester's language, then wait for explicit confirmation before changing specifications or implementing anything.
5. After confirmation, use `.agents/skills/specification-documentation/SKILL.md` to record approved decisions.

## Boundaries

- Do not edit specifications, decision records, or the roadmap during discovery.
- Do not create application code.
- Treat `docs/specs/` and its accepted decisions as the source of truth. Preserve confirmed decisions and leave open questions unresolved until confirmed.
- Do not repeat product requirements here; consult the relevant specification when needed.
