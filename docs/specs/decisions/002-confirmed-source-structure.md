# ADR-002: Confirmed Source Structure

**Status:** Accepted
**Date:** 2026-10-29

## Context

Agents need an approved source layout to use as the reference when planning or implementing changes. The requester specified the directory structure.

## Options considered

1. Use the source layout specified by the requester.
2. Leave the source layout undecided.

## Decision

Use the directory structure documented in [02 — Architecture](../02-architecture.md#confirmed-source-structure). The approved top-level source areas are `Web`, `Application`, `Domain`, and `Infrastructure`, with the listed subdirectories and `Web/Program.cs`.

This decision confirms the layout only. It does not define responsibilities, dependency rules, or additional architectural behavior.

## Consequences

- **Benefits:** Agents have a consistent, explicit directory layout to follow.
- **Costs or risks:** New architectural areas or changes to the layout require confirmation to avoid diverging from the approved structure.
- **Follow-up actions:** Consult the architecture specification before creating or moving source files. No application code is created by this decision.
