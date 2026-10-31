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
- A username uniquely identifies an account; username normalization rules remain open.
- Each account has a username, display name, and email attribute, in addition to its credential, one role, and active/inactive state. Field requiredness and validation remain open.
- Inactive accounts cannot authenticate.
- The three initial roles remain fixed: `Administrador`, `Atendente`, and `Terapeuta`. Creating or editing role definitions and permissions is not included in the first delivery.
- The initial Administrator is provisioned using configuration supplied outside the source code. The account must change its configured password at first login. No actual credential or secret may be committed to the repository.
- When an Administrator creates an account, the system generates a temporary password, displays it once, and requires the new user to change it at first login.
- Password reset/recovery is deferred from the first delivery. Account deletion is not specified.
- The remaining patient, responsible-person, professional, anamnesis, attendance, and evolution features remain in the implementation backlog and are not removed from the confirmed product scope.

## Consequences

- **Benefits:** The first delivery provides an access path and administrative user management before business workflows are added, without expanding the initial role model.
- **Costs or risks:** Account field requiredness and validation, username normalization, password policy, bootstrap lifecycle details, password reset/recovery, and account deletion remain open. The setup-phase restriction in `AGENTS.md` still prohibits application code until explicitly changed.
- **Follow-up actions:** Track foundation, authentication, bootstrap, and account-management work in the first-delivery section of the implementation roadmap. Keep business features and deferred access recovery in the backlog.
