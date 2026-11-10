# AI Agent Guidelines

These guidelines apply to Copilot working in this repository.

## Choose the task skill

| Request | Skill | Boundary |
|---|---|---|
| Analyze a GitHub issue | `.agents/skills/open-issue-analysis/SKILL.md` | Analyze first; wait for approval before implementation. |
| Explore or specify a product change | `.agents/skills/specification/SKILL.md` | Produce a decision brief; do not edit specifications or code. |
| Analyze, specify, implement, and publish a third-party API integration | `.agents/skills/third-party-api-integration/SKILL.md` | Start from supplied API documentation; wait for plan approval before implementation and PR publication. |
| Record an approved product decision | `.agents/skills/specification-documentation/SKILL.md` | Update specifications and roadmap; do not implement application behavior. |
| Assess roadmap readiness or report remaining work | `.agents/skills/implementation-planning/SKILL.md` | Plan only; do not implement. |
| Implement selected roadmap items | `.agents/skills/implement-selected-items/SKILL.md` | Implement only the explicitly selected item IDs. |

Load only the skill relevant to the request. Each skill owns its task procedure.

## Project source of truth

- `docs/specs/` is the source of truth for product requirements, accepted decisions, and implementation status. Read the relevant documents before proposing or changing product behavior.
- Treat accepted decisions as constraints and `To be defined` fields as open. Do not invent requirements or resolve open questions without requester confirmation.
- Keep project-specific requirements in `docs/specs/`, not duplicated in skills. Skills should link to the relevant specifications.
- Follow the roadmap in `docs/specs/06-implementation-checklist.md`. Add approved work before implementation, implement only selected items, and mark work `DONE` only after relevant validation passes.

## Governance and precedence

- This repository supports only Copilot CLI agent guidance; other agent frameworks are out of scope unless explicitly added.
- `AGENTS.md` is the single repository entry point and the default selector for task routing.
- Skills are intentionally small and specialized. Each skill has a clear task boundary, expected outcome, and reviewable validation criteria.
- In case of conflict, precedence is: `docs/specs/` > `AGENTS.md` > skill instructions.
- Product requirements and accepted decisions in `docs/specs/` take precedence over any repository guidance that would narrow or redefine confirmed scope.
- Duplicate or stale instructions must not remain in force without explicit review. Outdated guidance is removed only after verification and a documented review.
- A skill is considered valid only when it exists in `.agents/skills/`, matches a single dedicated task, and passes discovery/compatibility validation for the Copilot CLI.
- Skills are reviewed before they are considered ready for routine use.

## Confirmed project constraints

- Follow the confirmed stack, source layout, dependency direction, and modular-monolith principles in `docs/specs/02-architecture.md` and the accepted architecture decisions. Do not introduce unnecessary infrastructure or change confirmed project boundaries without approval.
- Follow the responsive layout, Bootstrap 5, and visual-system requirements in `docs/specs/03-ux-and-accessibility.md`. Do not invent unspecified breakpoints, interactions, or accessibility conformance.
- Preserve the local Docker requirements in `docs/specs/02-architecture.md`; they do not define production deployment.
- Follow product scope, roles, permissions, and lifecycle rules in `docs/specs/00-product-brief.md`, `docs/specs/05-use-cases.md`, and their accepted decisions. Do not infer open data rules, privacy requirements, or behavior.
- Favor clean, cohesive, maintainable solutions. Avoid unnecessary duplication and complexity.

## Safety and collaboration

- Ask before making a decision that could change scope, user experience, data, security, architecture, or cost.
- Preserve unrelated work. Do not expose credentials, tokens, or personal data in prompts or documentation.
