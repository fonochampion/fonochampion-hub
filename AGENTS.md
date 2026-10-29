# AI Agent Guidelines

These guidelines are shared by Copilot, Antigravity, and any other agent used in this repository. The specifications in `docs/specs/` are the source of truth for the product.

## Before starting work

- Read these guidelines and the `docs/specs/` documents relevant to the task.
- Treat confirmed specifications and accepted decisions as constraints. Treat `To be defined` fields and open decisions as unknown; do not invent requirements, product domain, undecided technologies, or behavior.
- Follow the confirmed architectural principles: modular monolith, simplicity, low operational cost, maintainability, responsiveness, and future evolvability. Do not introduce microservices, Kubernetes, messaging infrastructure, distributed cache, or other unnecessary components without an explicit confirmed requirement and requester approval.
- Follow the confirmed responsive UI specification: Bootstrap 5, with a header, collapsible navigation menu, content area, and footer, supporting desktop, tablet, and smartphone viewports. Do not invent exact breakpoints or menu interactions beyond the specs.
- Follow the confirmed visual color system in `docs/specs/03-ux-and-accessibility.md`, including token values, intended use, light/dark theme roles, and the approximate 60/30/10 color balance. Treat reference images as chromatic references only; do not copy their layout, components, or structure. Validate contrast, and do not assume an accessibility conformance level not specified there.
- Follow the confirmed Docker local development requirement: provide a root `Dockerfile` for the ASP.NET Core application and a root `docker-compose.yml` for running it with PostgreSQL locally. This does not imply production containerization; do not invent unspecified Docker settings as confirmed requirements.
- The confirmed initial product scope includes patients and responsible persons, optional dated anamnesis records with preserved revisions, completed patient attendance/evolution records, and professional registration initially limited to fonoaudiólogos: unique CPF, certifications (description, alphanumeric registration, completion date), telephone/email contacts, and create/modify/inactivate/delete flows. During patient registration, association with zero or more existing, active professionals is optional; an active professional may also be added later through the patient record, and a professional may be associated with multiple patients. An attendance requires at least one active professional already associated with the patient; it cannot create that association. A professional associated with a patient cannot be deleted. Professional council information via public API and appointment scheduling are deferred. Anamnesis and attendance access controls, privacy, retention, detailed prompts, and revision metadata remain open; do not implement related behavior until specified. Consult `docs/specs/05-use-cases.md`; do not infer open fields, validations, responsible-person/contact cardinality, contact/certification requiredness or multiplicity, CPF validation, association removal behavior, inactivation effects, attendance deletion/cancellation, or other features.
- The initial scope also includes completed patient attendance records, not appointment scheduling. Each attendance concerns one patient and one or more active professionals associated with the patient; each participating professional has an optional evolution entry, and updates preserve prior versions. Attending professionals can record or update evolution entries. Until scheduling exists, date/time may be absent; in the future every attendance must link to an appointment and obtain date/time from it. Attendance/evolution access controls, privacy, retention, revision metadata, and cancellation/deletion behavior remain open; do not invent them. Consult `docs/specs/05-use-cases.md`.
- Use `docs/specs/06-implementation-checklist.md` as the master implementation roadmap. When asked what remains, report pending, blocked, and in-progress items in its hierarchy, exclude done items, and explain blockers. Update item status during implementation; mark an item done only after its implementation and relevant validation pass. Add approved new work to the roadmap before implementing it, without inventing scope or changing completed history.
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
