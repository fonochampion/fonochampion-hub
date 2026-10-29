# AI Agent Guidelines

These guidelines are shared by Copilot, Antigravity, and any other agent used in this repository. The specifications in `docs/specs/` are the source of truth for the product.

## Before starting work

- Read these guidelines and the `docs/specs/` documents relevant to the task.
- Treat confirmed specifications and accepted decisions as constraints. Treat `To be defined` fields and open decisions as unknown; do not invent requirements, product domain, undecided technologies, or behavior.
- Follow the confirmed architectural principles: modular monolith, simplicity, low operational cost, maintainability, responsiveness, and future evolvability. Do not introduce microservices, Kubernetes, messaging infrastructure, distributed cache, or other unnecessary components without an explicit confirmed requirement and requester approval.
- Follow the confirmed responsive UI specification: Bootstrap 5, with a header, collapsible navigation menu, content area, and footer, supporting desktop, tablet, and smartphone viewports. Do not invent exact breakpoints or menu interactions beyond the specs.
- Follow the confirmed visual color system in `docs/specs/03-ux-and-accessibility.md`, including token values, intended use, light/dark theme roles, and the approximate 60/30/10 color balance. Treat reference images as chromatic references only; do not copy their layout, components, or structure. Validate contrast, and do not assume an accessibility conformance level not specified there.
- Follow the confirmed Docker local development requirement: provide a root `Dockerfile` for the ASP.NET Core application and a root `docker-compose.yml` for running it with PostgreSQL locally. This does not imply production containerization; do not invent unspecified Docker settings as confirmed requirements.
- The confirmed initial product scope is patients with full name, age, and CID; responsible persons associated with patients, with full name and degree of kinship; and telephone and email contact details for responsible persons. CID retrieval by public API is planned for a later stage. Consult `docs/specs/05-use-cases.md`; do not infer age representation, kinship values, pre-integration CID behavior, cardinality, requiredness, validation, record lifecycle, user permissions, scheduling, clinical records, or other features that remain open.
- If a decision could change scope, user experience, data, security, or cost, ask for confirmation before marking it as decided.
- During this setup phase, work only on agent artifacts and specifications. Do not create application code.

## Maintaining specifications

- Write in concise, verifiable English.
- Distinguish confirmed requirements, assumptions, and open questions.
- Give requirements and acceptance criteria stable identifiers.
- Record significant decisions in `docs/specs/decisions/`, including context, decision, and consequences.
- Update the index in `docs/specs/README.md` when adding or renaming documents.

## Using multiple tools

- Copilot and Antigravity should consult these guidelines and the specifications before proposing or performing work.
- When using Grok through its website/app, provide `AGENTS.md` and only the relevant specification documents as context. State the goal explicitly and ask it to preserve open questions rather than assume answers.
- Do not include credentials, tokens, or personal data in prompts or context documents.
