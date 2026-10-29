# ADR-006: Visual Color System

**Status:** Accepted
**Date:** 2026-10-29

## Context

The requester provided a color palette and usage guidance for the application's visual identity. A reference image is to inform chromatic identity only, not layout, components, or structure.

## Options considered

1. Use the supplied palette and usage guidance as the visual baseline.
2. Leave the color system unspecified.

## Decision

Use the palette, semantic colors, light and dark theme roles, and approximate 60/30/10 balance documented in [03 — UX and Accessibility](../03-ux-and-accessibility.md#color-system).

The visual direction is predominantly neutral surfaces with institutional violet, lilac secondary surfaces, and semantic green, coral/orange, red, and blue. The 60/30/10 proportions are guidance, not a strict per-screen metric. Do not copy any reference image's layout, components, or structure.

## Consequences

- **Benefits:** Establishes a consistent chromatic identity across themes and application surfaces.
- **Costs or risks:** Color combinations still require contrast validation; the required accessibility conformance level remains open.
- **Follow-up actions:** Use the UX specification as the source of truth for color tokens and their intended use. No interface implementation is part of this decision.
