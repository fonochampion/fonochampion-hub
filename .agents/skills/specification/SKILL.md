---
name: specification
description: "Use when discovering, drafting, reviewing, or updating product requirements, acceptance criteria, architecture, UX, data, or technical decisions before implementation."
---

# Product Specification

Help turn information from the requester into clear, traceable specifications. Keep specifications technology-agnostic where the stack is undecided, and preserve confirmed technology decisions as constraints.

## Procedure

1. Read `AGENTS.md` and the [specification index](../../../docs/specs/README.md).
2. Read relevant documents before proposing changes.
3. Identify gaps and conflicts. Ask about ambiguities that could change scope, user experience, data, security, or cost; do not fill those gaps yourself.
4. Distinguish confirmed information from assumptions and open questions. Treat accepted decisions in `docs/specs/decisions/` and confirmed architecture choices as constraints; do not replace them or infer unspecified choices.
5. For product scope, define stable, traceable use cases and derive functional requirements with verifiable acceptance criteria. Preserve explicitly confirmed fields and relationships; keep unspecified actors, domains, cardinalities, requiredness, validations, and lifecycle behavior open.
6. Update the corresponding product brief, requirements, use-case, UX, data, or architecture document and index when needed.
7. Record significant decisions in `docs/specs/decisions/`, including context and consequences.
8. Check document consistency and summarize changes and outstanding questions.

## Boundaries

- Do not create application code during specification discovery or review.
- Do not assume the product domain, undecided tech stack, integrations, architecture, or non-functional requirements.
- Preserve the confirmed stack in `docs/specs/02-architecture.md` unless the requester explicitly changes it.
- Preserve the confirmed source layout in `docs/specs/02-architecture.md`; do not change its named projects or directories without confirmation.
- Preserve the confirmed architectural principles: modular monolith, simplicity, low operational cost, maintainability, responsiveness, and future evolvability. Do not propose microservices, Kubernetes, messaging infrastructure, distributed cache, or other unnecessary components without an explicit confirmed requirement and requester approval.
- Preserve the confirmed responsive UI requirements in `docs/specs/03-ux-and-accessibility.md`: Bootstrap 5, header, collapsible navigation menu, content area, footer, and desktop/tablet/smartphone support. Keep exact breakpoints and menu interactions open unless confirmed.
- Preserve the confirmed visual color system in `docs/specs/03-ux-and-accessibility.md`, including its tokens, intended uses, light/dark roles, and approximate 60/30/10 balance. Use images only as chromatic references; do not copy layout, components, or structure. Keep unspecified visual and accessibility decisions open.
- Preserve the confirmed local Docker requirement in `docs/specs/02-architecture.md`: root `Dockerfile` for the ASP.NET Core application and root `docker-compose.yml` for local execution with PostgreSQL. Do not infer production containerization or unspecified Docker configuration.
- Preserve the confirmed initial product scope in `docs/specs/00-product-brief.md` and `docs/specs/05-use-cases.md`: patients have full name, age, and CID; each patient has an associated list of responsible persons with full name and degree of kinship; responsible persons have telephone and email contacts. CID retrieval through a public API is planned for a later stage. Do not invent the kinship domain, age representation, pre-integration CID behavior, validations, cardinality rules, contact requiredness/multiplicity, permissions, or record lifecycle.
- Do not mark an assumption as a decision without explicit confirmation.
