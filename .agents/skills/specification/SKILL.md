---
name: specification
description: "Use when discovering, drafting, reviewing, or updating product requirements, acceptance criteria, architecture, UX, data, or technical decisions before implementation."
---

# Product Specification Discovery

Analyze a proposed feature or change and prepare a decision brief for the requester. Keep specifications technology-agnostic where the stack is undecided, and preserve confirmed technology decisions as constraints. This is the discovery and decision stage; do not formalize an unconfirmed proposal in the specifications or implementation roadmap.

## Procedure

1. Read `AGENTS.md` and the [specification index](../../../docs/specs/README.md).
2. Read relevant specifications, accepted decisions, and roadmap items before assessing the proposal.
3. Identify conflicts, dependencies, and unanswered questions. Do not fill gaps yourself. Ask about ambiguities that could change scope, user experience, data, security, architecture, or cost.
4. Distinguish confirmed information from assumptions and open questions. Treat accepted decisions in `docs/specs/decisions/` and confirmed architecture choices as constraints; do not replace them or infer unspecified choices.
5. Prepare a concise decision brief: proposed feature/change, confirmed facts, alternatives or unresolved choices, consequences and dependencies, affected specifications, and the implementation work likely needed if approved. For product scope, outline traceable use cases and verifiable acceptance criteria while leaving unspecified actors, domains, cardinalities, requiredness, validations, and lifecycle behavior open.
6. Present the analysis to the requester and wait for explicit confirmation of the decisions or scope before making normative documentation changes. Use the `ask_user` tool when a decision is needed. A request to analyze is not approval to document or implement.
7. After confirmation, use the `specification-documentation` skill to update the relevant specifications and implementation roadmap. Record significant confirmed decisions in `docs/specs/decisions/`, including context and consequences.

## Boundaries

- Do not update specifications, decision records, or the implementation checklist with a proposal that has not been explicitly confirmed.
- Do not create application code during specification discovery or review.
- Do not assume the product domain, undecided tech stack, integrations, architecture, or non-functional requirements.
- Preserve the confirmed stack in `docs/specs/02-architecture.md` unless the requester explicitly changes it.
- Preserve the confirmed source layout in `docs/specs/02-architecture.md`; do not change its named projects or directories without confirmation.
- Preserve the confirmed architectural principles: modular monolith, simplicity, low operational cost, maintainability, responsiveness, and future evolvability. Do not propose microservices, Kubernetes, messaging infrastructure, distributed cache, or other unnecessary components without an explicit confirmed requirement and requester approval.
- Preserve the confirmed responsive UI requirements in `docs/specs/03-ux-and-accessibility.md`: Bootstrap 5, header, collapsible navigation menu, content area, footer, and desktop/tablet/smartphone support. Keep exact breakpoints and menu interactions open unless confirmed.
- Preserve the confirmed visual color system in `docs/specs/03-ux-and-accessibility.md`, including its tokens, intended uses, light/dark roles, and approximate 60/30/10 balance. Use images only as chromatic references; do not copy layout, components, or structure. Keep unspecified visual and accessibility decisions open.
- Preserve the confirmed local Docker requirement in `docs/specs/02-architecture.md`: root `Dockerfile` for the ASP.NET Core application and root `docker-compose.yml` for local execution with PostgreSQL. Do not infer production containerization or unspecified Docker configuration.
- Preserve the confirmed product scope in `docs/specs/00-product-brief.md` and `docs/specs/05-use-cases.md`, including patients and responsible persons, optional dated anamnesis records, scheduled and realized attendances/evolutions, and professional registration. Appointment scheduling is part of the confirmed product scope but is not in the first implementation delivery; public API retrieval of CID and professional council information remains deferred. The first implementation delivery and confirmed account/authentication decisions are recorded in ADR-019, while remaining business features stay in the roadmap backlog. Do not invent open fields, validations, privacy or retention requirements, or lifecycle behavior.
- Do not mark an assumption as a decision without explicit confirmation.
