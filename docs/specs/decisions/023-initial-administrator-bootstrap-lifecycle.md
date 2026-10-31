# ADR-023: Initial Administrator Bootstrap Lifecycle

**Status:** Accepted
**Date:** 2026-10-31

## Context

IMP-022 needs a repeatable, safe lifecycle for provisioning the initial Administrator from external configuration. The behavior when the configured account already exists or its username/password configuration changes was open.

## Options considered

1. Make bootstrap idempotent for a matching Administrator account and fail explicitly on identity conflicts or a changed username.
2. Synchronize configured credentials into an existing account on startup.
3. Create a new Administrator whenever the configured username does not match an existing account.

## Decision

- On startup, if the normalized configured username matches an existing account with the `Administrador` role, continue without changing any account state, including its password, active state, or password-change requirement. Changes to the configured password do not overwrite the existing credential.
- If the configured username matches an existing account without the `Administrador` role, fail startup without changing that account.
- If an `Administrador` account exists but none matches the normalized configured username, fail startup. Do not create another Administrator; a configured username change after initial provisioning requires explicit manual resolution.
- If neither the configured username nor an Administrator account exists, create the initial account with the configured username and password, assign the `Administrador` role, mark it active, and require a password change at first login.
- Username matching follows the trimming and case-insensitive comparison policy in [ADR-022](./022-initial-authentication-policy.md).

## Consequences

- **Benefits:** Repeated startup does not reset an Administrator's password or reactivate an account, and configuration drift cannot silently create duplicate Administrator accounts.
- **Costs or risks:** Changing the configured username after provisioning stops startup until an operator resolves the account/configuration mismatch. Password changes in configuration do not reset an existing account.
- **Follow-up actions:** Implement IMP-022 according to this lifecycle. Access recovery and account deletion remain outside this decision; account-field rules are specified in [ADR-024](./024-system-user-account-field-rules.md).
