# ADR-004: Responsive Standard Layout

**Status:** Accepted
**Date:** 2026-10-29

## Context

The application needs a consistent responsive interface. The requester specified Bootstrap 5, the standard page regions, and supported device classes.

## Options considered

1. Use the specified Bootstrap 5 responsive layout.
2. Leave the standard layout and device support unspecified.

## Decision

Use Bootstrap 5 and provide a standard layout containing:

- Header
- Collapsible navigation menu
- Content area
- Footer

Support desktop, tablet, and smartphone viewports. Exact breakpoints and menu interaction details remain open and must not be invented as product requirements.

## Consequences

- **Benefits:** Provides a consistent page structure and responsive experience across the specified device classes.
- **Costs or risks:** The layout must be validated across desktop, tablet, and smartphone sizes; exact breakpoint and interaction choices remain to be made during design or implementation.
- **Follow-up actions:** Use [03 — UX and Accessibility](../03-ux-and-accessibility.md) as the implementation reference. Do not create application code as part of this decision.
