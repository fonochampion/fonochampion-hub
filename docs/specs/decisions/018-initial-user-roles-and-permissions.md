# ADR-018: Initial User Roles and Permissions

**Status:** Accepted
**Date:** 2026-10-30

## Context

The initial specifications left authentication, user roles, and authorization open. The requester defined three roles and their responsibilities, including access to clinical information and account administration. These rules also needed reconciliation with existing attendance participation and evolution-authorship constraints.

## Options considered

1. Leave role permissions unspecified until implementation.
2. Define the initial role set and map confirmed product operations to each role while keeping authentication and account details open.

## Decision

- Each system-user account has exactly one initial role: `Administrador` (Administrator), `Atendente` (Attendant), or `Terapeuta` (Therapist).
- `Administrador` can perform all specified system operations, including patient and professional maintenance, account management, clinical-data access, and cancellation-reason maintenance. An Administrator may cancel any `Agendado` attendance, even without participating, but may start or end an attendance only when participating. The Administrator cannot create or update another professional's evolution; each professional's evolution remains restricted to its participating author.
- `Atendente` maintains patient records and patient-professional associations, responsible persons and their contact details, attendance scheduling and rescheduling, and attendance participants subject to the UC-008 status and participant constraints. The Attendant may access operational scheduling data but cannot access anamnesis or evolution content and cannot cancel an attendance.
- `Terapeuta` may access patient anamneses, attendances, and evolutions for all patients and may maintain anamneses. A Therapist may start, end, or cancel an attendance only when participating and when allowed by its status, and may create or update only their own evolution entry. Therapists do not schedule attendances or maintain attendance participants.
- Only Administrators may create or edit system-user accounts, assign or change a role, activate or inactivate an account, and initiate an access reset. Account deletion is not specified in the initial scope.
- Initial-delivery authentication, account attributes, fixed-role usage, temporary-password flow, and Administrator bootstrap are defined in [ADR-019](./019-first-delivery-authentication-and-user-management.md). Account-field rules, username normalization, password policy, reset mechanics, and account deletion remain open.

## Consequences

- **Benefits:** Initial access responsibilities are traceable across patient operations, clinical information, attendance scheduling, professional management, cancellation reasons, and account management.
- **Costs or risks:** Account-field rules, username normalization, password policy, reset/recovery, and deletion remain unresolved; the Administrator's broad access requires the remaining privacy and legal/data-handling requirements to be specified.
- **Follow-up actions:** Implement the first-delivery access-management flow according to ADR-019. Apply the role matrix to business operations when those features are implemented, preserving applicable privacy requirements and the ownership and participation constraints in UC-008.
