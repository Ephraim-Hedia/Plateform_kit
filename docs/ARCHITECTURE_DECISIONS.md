# ARCHITECTURE_DECISIONS.md

## Purpose

This document records architectural decisions that have been approved for the Platform starter kit.

These decisions are considered authoritative.

Unless explicitly changed, all future implementation must follow these decisions.

---

# ADR-001

Decision:
Use Clean Architecture.

Status:
Approved

Reason:
Provides clear separation of concerns, maintainability, testability, and long-term scalability.

---

# ADR-002

Decision:
Use CQRS with MediatR.

Status:
Approved

Reason:
Provides clear separation between commands and queries and keeps use cases isolated and maintainable.

---

# ADR-003

Decision:
Use Feature-Based Folder Organization.

Status:
Approved

Example:

Authentication/
├── Login/
├── Register/
├── Logout/
└── RefreshToken/

Reason:
Keeps all files related to a use case together and improves maintainability.

---

# ADR-004

Decision:
Use Entity Framework Core.

Status:
Approved

Version:
Entity Framework Core 9

Reason:
Provides a mature ORM with strong support for SQL Server, migrations, interceptors, and query filtering.

---

# ADR-005

Decision:
Avoid Generic Repository Pattern.

Status:
Approved

Reason:
DbContext already provides Repository and Unit of Work behavior.

Adding a generic repository introduces unnecessary abstraction and complexity.

Prefer direct DbContext usage.

Repositories may be introduced only when they provide clear value.

---

# ADR-006

Decision:
Use SQL Server.

Status:
Approved

Version:
SQL Server 2022

Reason:
Primary database provider for the platform.

Database-specific implementation must remain isolated to Infrastructure.

Future providers may be added without changing Domain or Application.

---

# ADR-007

Decision:
Use FluentValidation.

Status:
Approved

Reason:
Provides centralized validation and integrates naturally with CQRS pipeline behaviors.

---

# ADR-008

Decision:
Use Permission-Based Authorization.

Status:
Approved

Reason:
Provides finer-grained access control than role-only authorization.

Roles act as containers of permissions.

Permissions represent the actual authorization boundary.

---

# ADR-009

Decision:
Use JWT Access Tokens.

Status:
Approved

Reason:
Provides stateless authentication suitable for APIs and distributed systems.

---

# ADR-010

Decision:
Use Refresh Token Rotation.

Status:
Approved

Reason:
Improves security by invalidating previously issued refresh tokens whenever a new refresh token is issued.

---

# ADR-011

Decision:
Use ASP.NET Identity as the authentication foundation.

Status:
Approved

Identity Responsibilities:

* User Management
* Password Hashing
* Password Reset
* Email Confirmation
* Account Lockout
* User Tokens
* External Authentication

Reason:
Reduces custom security code and leverages Microsoft's battle-tested authentication infrastructure.

---

# ADR-012

Decision:
Use DateTimeOffset for persisted timestamps.

Status:
Approved

Reason:
Supports auditing, UTC storage, and future multi-timezone requirements.

Rules:

* Store all timestamps in UTC.
* Use DateTimeOffset.UtcNow.
* Never use DateTime.Now.

---

# ADR-013

Decision:
Controllers communicate only through MediatR.

Status:
Approved

Reason:
Prevents business logic from leaking into the API layer and enforces CQRS boundaries.

Controllers:

* Receive requests
* Send Commands/Queries
* Return responses

Controllers must not:

* Access DbContext directly
* Contain business logic

---

# ADR-014

Decision:
Use Serilog for logging.

Status:
Approved

Required Sinks:

* Console
* File

Reason:
Provides structured logging and strong ecosystem support.

---

# ADR-015

Decision:
Architecture Tests are mandatory.

Status:
Approved

Must Verify:

* Domain → Infrastructure dependency forbidden
* Domain → API dependency forbidden
* Application → Infrastructure dependency forbidden
* Application → API dependency forbidden

Reason:
Protects architectural boundaries from accidental violations.

---

# ADR-016

Decision:
Use Guid as the default identifier type.

Status:
Approved

Implementation:

Entity<TId>

Platform entities use Guid.

Reason:
Keeps the platform simple while allowing future projects to adopt strongly typed identifiers if needed.

---

# ADR-017

Decision:
Use AutoMapper for object mapping.

Status:
Approved

Reason:
Mature ecosystem, widespread adoption, extensive documentation, and familiarity among .NET developers.

---

# ADR-018

Decision:
Use NetArchTest for architecture testing.

Status:
Approved

Reason:
Provides a simple mechanism for enforcing architectural rules through automated tests.

---

# ADR-019

Decision:
Support Domain Events from Day One.

Status:
Approved

Implementation:

* IDomainEvent
* AggregateRoot<TId>

Initial Scope:

* Domain Event abstractions only
* No publishing infrastructure initially

Reason:
Allows future event-driven features without modifying the Domain Foundation.

---

# ADR-020

Decision:
Only Aggregate Roots may raise Domain Events.

Status:
Approved

Implementation:

AggregateRoot<TId> contains Domain Event support.

Regular entities do not contain Domain Event collections.

Reason:
Follows Domain-Driven Design principles and keeps event ownership at aggregate boundaries.

---

# ADR-021

Decision:
Use Result Pattern for expected failures.

Status:
Approved

Implementation:

* Result
* Result<T>

Reason:
Expected business failures should not be represented by exceptions.

Examples:

* User not found
* Invalid credentials
* Permission denied
* Duplicate email

Unexpected failures may still throw exceptions.

---

# ADR-022

Decision:
Result contains a single Error.

Status:
Approved

Implementation:

Result
{
Error Error
}

Reason:
Most business failures contain a single reason.

Validation failures are handled separately through FluentValidation.

---

# ADR-023

Decision:
Use ErrorType classification.

Status:
Approved

Supported Types:

* Validation
* NotFound
* Conflict
* Unauthorized
* Forbidden
* Failure

Reason:
Allows consistent mapping from business failures to HTTP status codes.

---

# ADR-024

Decision:
Use AuditableEntity<TId>.

Status:
Approved

Implementation:

CreatedAt
UpdatedAt
CreatedBy
UpdatedBy

Reason:
Most business systems require auditing.

Auditing should be available from the platform foundation.

---

# ADR-025

Decision:
Support Soft Delete from Day One.

Status:
Approved

Implementation:

ISoftDeletable

Properties:

* IsDeleted
* DeletedAt

Reason:
Soft delete is a common business requirement and is difficult to retrofit later.

---

# ADR-026

Decision:
Use Value Object foundation.

Status:
Approved

Implementation:

ValueObject base class.

Examples:

* Email
* PhoneNumber
* Money
* Address
* Percentage

Reason:
Provides a foundation for future domain modeling needs.

---

# ADR-027

Decision:
Use ICurrentUser abstraction.

Status:
Approved

Reason:
Application layer must not depend on ASP.NET Core or HttpContext.

Implementation:

Application:

ICurrentUser

Infrastructure:

CurrentUser using IHttpContextAccessor

---

# ADR-028

Decision:
Use custom RefreshToken entity.

Status:
Approved

Do Not Use:

Identity User Tokens for refresh token management.

Requirements:

* Revocation
* Expiration
* Rotation
* Multi-device support

Reason:
Provides greater flexibility and control over token lifecycle management.

---

# ADR-029

Decision:
Store permissions in a dedicated Permission table.

Status:
Approved

Reason:
Permissions are business concepts and should be managed independently of Identity Claims.

Supports:

* Permission management screens
* Reporting
* Permission metadata
* Future extensibility

---

# ADR-030

Decision:
Use Identity Roles with custom Permission relationships.

Status:
Approved

Architecture:

ApplicationUser
↓
ApplicationRole
↓
RolePermission
↓
Permission

Identity manages:

* Users
* Roles

Platform manages:

* Permissions
* RolePermissions

Reason:
Combines Identity's mature user and role management with a flexible permission model.

---

# ADR-031

Decision:
JWT tokens contain Roles but not Permissions.

Status:
Approved

JWT Contains:

* UserId
* Email
* Roles

JWT Does Not Contain:

* Permissions

Reason:
Allows permission changes to take effect immediately without waiting for token expiration.

---

# ADR-032

Decision:
Authorization uses IMemoryCache.

Status:
Approved

Cache:

* Roles
* Permissions
* RolePermission mappings

Do Not Cache:

* Passwords
* JWT Tokens
* Refresh Tokens

Reason:
Provides fast authorization checks without requiring Redis.

Suitable for the platform foundation and single-instance deployments.

---

# ADR-033

Decision:
ApplicationUser and ApplicationRole belong to Infrastructure.

Status:
Approved

Implementation:

Infrastructure/
└── Identity/
├── ApplicationUser.cs
└── ApplicationRole.cs

Reason:
ASP.NET Identity is an infrastructure concern.

Keeping Identity models in Infrastructure preserves Domain independence and reduces framework coupling.

---

# ADR-034

Decision:
Use IMemoryCache for Role → Permission lookups.

Status:
Approved

Authorization Flow:

Request
↓
JWT
↓
Roles
↓
IMemoryCache
↓
Permissions
↓
Authorization

Reason:
Eliminates database queries during authorization while allowing permission changes to take effect immediately after cache invalidation.

---

# ADR-035

Decision:
The Domain layer must not reference ASP.NET Identity.

Status:
Approved

Reason:
Authentication implementation details should remain outside the Domain layer.

Domain should remain independent of framework-specific identity implementations.

---

# ADR-036

Decision:
The Platform Foundation prioritizes simplicity over maximum extensibility.

Status:
Approved

Guideline:

Approximately:

* 70% Simplicity
* 30% Extensibility

Reason:
The platform is intended for practical reuse by a small team or individual developer.

Avoid over-engineering and speculative abstractions.

---

# ADR-037

Decision:
Use MediatR Pipeline Behaviors for cross-cutting concerns.

Status:
Approved

Implementation:

* LoggingBehavior<TRequest, TResponse>
* ValidationBehavior<TRequest, TResponse>

Registered as open generic behaviors in Application's AddApplicationServices,
in the order Logging -> Validation -> Handler.

Reason:
Keeps cross-cutting concerns (logging, validation) out of individual command
and query handlers while remaining fully within the Application layer.

---

# ADR-038

Decision:
ValidationBehavior returns ValidationResult / ValidationResult<T> instead of throwing.

Status:
Approved

Implementation:

* IValidationResult (Domain)
* ValidationResult : Result, IValidationResult
* ValidationResult<TValue> : Result<TValue>, IValidationResult

ValidationBehavior<TRequest, TResponse> requires TResponse : Result, runs all
registered FluentValidation validators for the request, and on failure returns
a ValidationResult (or ValidationResult<TValue>) carrying every Error produced
by FluentValidation, without calling the handler.

Reason:
Keeps validation failures inside the Result pattern (ADR-021/022) - expected
failures still do not throw - while still exposing the full set of per-field
validation errors to the API layer, instead of being limited to a single Error.

---

# ADR-039

Decision:
Centralize Result -> HTTP mapping in ApiControllerBase, and unexpected
exceptions in a global IExceptionHandler.

Status:
Approved

Implementation:

API/Common/ApiControllerBase:

* HandleFailure(Result result) maps ErrorType to ProblemDetails / status codes.
* IValidationResult results are mapped to ValidationProblemDetails (400) with
  one entry per Error.

API/Common/GlobalExceptionHandler (IExceptionHandler):

* Catches unhandled exceptions, logs them, and returns a generic 500
  ProblemDetails response.

Reason:
Keeps controllers thin (ADR-013) and ensures consistent ProblemDetails
responses for both expected (Result) and unexpected (exception) failures.

---

# ADR-040

Decision:
Configure Serilog from appsettings via Serilog.Settings.Configuration, with
Console and File sinks, plus Serilog request logging middleware.

Status:
Approved

Implementation:

* Program.cs: builder.Host.UseSerilog((context, configuration) =>
  configuration.ReadFrom.Configuration(context.Configuration))
* app.UseSerilogRequestLogging()
* appsettings.json / appsettings.Development.json: Serilog section
  (MinimumLevel, WriteTo: Console + File, Enrich: FromLogContext)

Reason:
Satisfies the Logging requirements (ADR-014) with structured, configurable
logging and per-request logging out of the box.

---

# ADR-041

Decision:
Define the permission catalog as Domain constants, and seed the catalog plus
a baseline Administrator role via EF Core HasData.

Status:
Approved

Implementation:

* Platform.Domain.Constants.Permissions: nested static classes (Roles,
  PermissionCatalog, ...) holding permission code strings (e.g.
  Roles.View, Roles.Manage, PermissionCatalog.View), plus an All list.
* Platform.Infrastructure.Persistence.Seed.AuthorizationSeedData: HasData
  for the Administrator ApplicationRole, the Permission rows (one per
  Permissions.* constant), and the RolePermission rows granting all
  catalog permissions to Administrator.

Reason:
Permission codes referenced by [HasPermission("...")] (ADR-030) and by seed
data must stay in sync; defining them once as Domain constants gives
compile-time checking for both. HasData ensures every migrated database
starts with a usable Administrator role that can manage roles and
permissions, without a separate manual seeding step.

---

# ADR-042

Decision:
RolePermission has its own surrogate Guid Id rather than a composite key of
(RoleId, PermissionId).

Status:
Approved

Implementation:

* RolePermission : Entity<Guid>, created via RolePermission.Create(roleId,
  permissionId).
* RolePermissionConfiguration: HasKey(Id), plus a unique index on
  (RoleId, PermissionId) to enforce one mapping per role/permission pair.

Reason:
Keeps RolePermission consistent with the Entity<Guid> foundation (every
entity has a single Guid identity) and simplifies HasData seeding, while the
unique index preserves the same uniqueness guarantee a composite key would
provide.

---

# ADR-043

Decision:
Implement permission checks via a dynamic "Permission:" authorization policy
provider, an AuthorizationHandler<PermissionRequirement>, and a
[HasPermission("code")] attribute.

Status:
Approved

Implementation:

* PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
  recognizes policy names prefixed with "Permission:" and builds an
  AuthorizationPolicy containing a PermissionRequirement(code) on demand,
  falling back to the default provider for all other policy names.
* PermissionAuthorizationHandler reads the caller's roles from the
  ClaimTypes.Role claims (ADR-031), resolves the role -> permission set via
  IPermissionService (IMemoryCache, ADR-034), and succeeds the requirement
  if the permission is present.
* HasPermissionAttribute(string permission) : AuthorizeAttribute is sugar
  for [Authorize(Policy = "Permission:" + permission)].

Reason:
A static AuthorizationPolicy would need to be registered up front for every
permission code, requiring central updates each time a new permission is
added. Resolving policies dynamically lets controllers declare
[HasPermission("Products.Create")] (ADR-030's preferred usage) for any
permission code without further registration.

---

# ADR-044

Decision:
Introduce IPermissionCacheInvalidator as an Application abstraction,
implemented in Infrastructure over IMemoryCache, and call it whenever a
role's permission assignments change.

Status:
Approved

Implementation:

* Platform.Application.Abstractions.IPermissionCacheInvalidator:
  InvalidateRole(string roleName).
* Platform.Infrastructure.Authorization.PermissionCacheInvalidator removes
  the cached permission set for that role name (same cache/key scheme used
  by PermissionService).
* AssignPermissionsCommandHandler calls InvalidateRole(role.Name) after
  persisting the new RolePermission set for a role.

Reason:
Implements the mandatory cache invalidation rule from ADR-032/034 ("cache
must be invalidated whenever a role-permission assignment changes") without
giving the Application layer a direct dependency on IMemoryCache, which
remains an Infrastructure concern.
