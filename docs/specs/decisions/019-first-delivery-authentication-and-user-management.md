# ADR-019: First Delivery Authentication and User Management

**Status:** Accepted
**Date:** 2026-10-30

## Context

The requester selected authentication and system-user management as the first implementation delivery, with business-domain features remaining in the implementation backlog. The project already defines three initial roles and their responsibilities, but authentication and initial Administrator provisioning were open.

## Options considered

1. Use an external identity provider or email-based authentication.
2. Use local username/password authentication and keep the already-defined roles fixed.

## Decision

- The first implementation delivery includes the application foundation, local username/password authentication, initial Administrator provisioning, and system-user account management.
- Email is an account attribute and is not used as the authentication identifier.
- A username uniquely identifies an account. Username normalization is defined in [ADR-022](./022-initial-authentication-policy.md).
- Each account has a username, display name, and email attribute, in addition to its credential, one role, and active/inactive state. Display-name and email rules are defined in [ADR-024](./024-system-user-account-field-rules.md).
- Inactive accounts cannot authenticate.
- The three initial roles remain fixed: `Administrador`, `Atendente`, and `Terapeuta`. Creating or editing role definitions and permissions is not included in the first delivery.
- The initial Administrator is provisioned using configuration supplied outside the source code. The account must change its configured password at first login. The bootstrap lifecycle is defined in [ADR-023](./023-initial-administrator-bootstrap-lifecycle.md). No actual credential or secret may be committed to the repository.
- When an Administrator creates an account, the system generates a temporary password, displays it once, and requires the new user to change it at first login.
- Password reset/recovery is deferred from the first delivery. Account deletion is not specified.
- The remaining patient, responsible-person, professional, anamnesis, attendance, and evolution features remain in the implementation backlog and are not removed from the confirmed product scope.

## Consequences

- **Benefits:** The first delivery provides an access path and administrative user management before business workflows are added, without expanding the initial role model.
- **Costs or risks:** Password reset/recovery and account deletion remain open. Username, password, session, and failed-login policies are defined in [ADR-022](./022-initial-authentication-policy.md).
- **Follow-up actions:** Track foundation, authentication, bootstrap, and account-management work in the first-delivery section of the implementation roadmap. Authentication, password, and session policies are specified in [ADR-022](./022-initial-authentication-policy.md), bootstrap behavior in [ADR-023](./023-initial-administrator-bootstrap-lifecycle.md), and account-field rules in [ADR-024](./024-system-user-account-field-rules.md). Keep business features and deferred access recovery in the backlog.
