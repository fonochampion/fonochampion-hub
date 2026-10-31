# ADR-022: Initial Authentication Policy

**Status:** Accepted
**Date:** 2026-10-31

## Context

IMP-021 requires implementable username/password authentication, authenticated-session behavior, and the mandatory first-login password change already confirmed in ADR-019. Username normalization, password strength, session expiration, and repeated-failure behavior were still open.

## Options considered

1. Define explicit local username, password, session, and failed-login policies for the first delivery.
2. Leave these authentication policies open until implementation.

## Decision

- Trim leading and trailing whitespace from usernames and compare usernames case-insensitively for uniqueness and authentication.
- User-set passwords must contain at least 12 characters. No character-composition rules are required. Apply this policy when changing the initial Administrator password and a temporary password.
- An authenticated session expires after 30 minutes of inactivity.
- Five consecutive failed login attempts for the same normalized username cause a 15-minute lockout.
- The failed-login counter resets when the lockout expires.
- A successful authentication resets the consecutive failed-login attempt count.
- Login failure messages must not reveal whether the username exists.
- Apply the same mandatory first-login password-change flow to the initial Administrator's configured password and to temporary passwords for Administrator-created accounts.

## Consequences

- **Benefits:** Defines observable authentication and session behavior for IMP-021 and a consistent first-login flow for IMP-020 and IMP-022.
- **Costs or risks:** A username-based lockout can be triggered by repeated attempts against a known username.
- **Follow-up actions:** Implement IMP-021 under these rules. The initial Administrator bootstrap lifecycle is specified in [ADR-023](./023-initial-administrator-bootstrap-lifecycle.md), and system-user account field rules in [ADR-024](./024-system-user-account-field-rules.md).
