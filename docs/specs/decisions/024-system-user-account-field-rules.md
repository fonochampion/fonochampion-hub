# ADR-024: System-User Account Field Rules

**Status:** Accepted
**Date:** 2026-10-31

## Context

IMP-020 requires implementable rules for the display name and email attributes on system-user accounts. Their requiredness and validation had not been defined.

## Options considered

1. Define minimal requiredness and format rules for the first delivery.
2. Leave account-field rules open and defer system-user account management.

## Decision

- A system-user display name is required. Trim leading and trailing whitespace and reject a value that is empty after trimming. No additional display-name format or length rules are specified.
- Email is optional. When provided, trim leading and trailing whitespace and validate its conventional email-address format. Email uniqueness and ownership verification are not required.

## Consequences

- **Benefits:** Defines the account-field behavior needed to implement IMP-020 without adding unspecified length, display-name format, uniqueness, or verification requirements.
- **Costs or risks:** These minimal rules do not establish email ownership or uniqueness; email remains an account attribute and is not an authentication identifier.
- **Follow-up actions:** Implement IMP-020 using these rules. Password reset/recovery and account deletion remain outside the first delivery.
