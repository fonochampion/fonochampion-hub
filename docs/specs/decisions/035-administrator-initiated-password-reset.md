# ADR-035: Administrator-Initiated Password Reset

**Status:** Accepted
**Date:** 2026-11-18

## Context

Password reset/recovery was deferred from the first implementation delivery. The later IMP-024 delivery requires a defined reset flow and security behavior. Accounts use local username/password authentication; email is optional and its ownership is not verified.

## Options considered

1. Let an Administrator initiate a reset by issuing a system-generated temporary password.
2. Provide self-service recovery through email.
3. Leave reset behavior unspecified until a later decision.

## Decision

- Only an Administrator may initiate a password reset.
- The system generates a temporary password, displays it to the Administrator once, and requires the user to change it at the next login.
- The temporary password expires 24 hours after issuance.
- Initiating a reset immediately revokes the account's existing authenticated sessions and clears its consecutive failed-login counter and any active lockout.
- An Administrator may reset an inactive account, but the reset does not reactivate it; the account remains unable to authenticate until separately reactivated.
- Reset does not use email or provide self-service recovery because email ownership is not verified.
- The new user-set password continues to follow the minimum 12-character policy in [ADR-022](./022-initial-authentication-policy.md).

## Consequences

- **Benefits:** Defines an Administrator-controlled reset flow without relying on unverified email ownership, while invalidating sessions and lockout state associated with the previous credential.
- **Costs or risks:** The Administrator must communicate the one-time temporary password to the account holder. If it is not used within 24 hours, another reset is required. Self-service recovery remains unavailable.
- **Follow-up actions:** Implement the confirmed behavior in IMP-024; this decision does not add the feature to the first implementation delivery.
