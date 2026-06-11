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

---

# ADR-045

Decision:
Expose an interactive OpenAPI UI in Development using Scalar.AspNetCore, on
top of ASP.NET Core's built-in OpenAPI document generation.

Status:
Approved

Implementation:

* Platform.API.csproj: Scalar.AspNetCore.
* Program.cs: builder.Services.AddOpenApi(); in
  app.Environment.IsDevelopment(), app.MapOpenApi() followed by
  app.MapScalarApiReference().

Reason:
Satisfies the "Use: Scalar" API rule with a lightweight reference UI driven
by the generated OpenAPI document, with no separate document-generation
package to maintain. Restricting it to Development avoids exposing API
documentation in other environments.

---

# ADR-046

Decision:
Split health checks into a liveness endpoint (/health/live) that runs no
checks, and a readiness endpoint (/health/ready) that runs checks tagged
"ready", including database connectivity.

Status:
Approved

Implementation:

* Platform.Infrastructure.csproj:
  Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore.
* Platform.Infrastructure.DependencyInjection.AddInfrastructureServices:
  services.AddHealthChecks()
      .AddDbContextCheck<ApplicationDbContext>("database", tags: ["ready"]).
* Program.cs:
  app.MapHealthChecks("/health/live", new HealthCheckOptions
  { Predicate = _ => false });
  app.MapHealthChecks("/health/ready", new HealthCheckOptions
  { Predicate = check => check.Tags.Contains("ready") });

Reason:
A liveness probe should be cheap and independent of downstream dependencies,
so a temporarily unreachable database does not cause an orchestrator to
restart an otherwise-healthy process. A readiness probe should reflect
whether the service can actually serve traffic, so it includes the database
check. The DB check is registered in Infrastructure alongside
ApplicationDbContext, keeping each layer responsible for its own service
registration (Dependency Injection rules).

---

# ADR-047

Decision:
Version the API using URL segments (/api/v1/...) via Asp.Versioning.Mvc and
Asp.Versioning.Mvc.ApiExplorer, with [ApiVersion("1.0")] and
[Route("api/v{version:apiVersion}/...")] on every controller.

Status:
Approved

Implementation:

* Platform.API.csproj: Asp.Versioning.Mvc 8.1.0,
  Asp.Versioning.Mvc.ApiExplorer 8.1.0 (the default 10.0.0 targets net10.0
  only and is incompatible with net9.0).
* Platform.API.DependencyInjection.AddApiServices:
  AddApiVersioning(options => { DefaultApiVersion = new ApiVersion(1, 0);
  AssumeDefaultVersionWhenUnspecified = true; ReportApiVersions = true;
  ApiVersionReader = new UrlSegmentApiVersionReader(); })
      .AddMvc()
      .AddApiExplorer(options => { GroupNameFormat = "'v'VVV";
      SubstituteApiVersionInUrl = true; }).
* AuthController, PermissionsController, RolesController:
  [ApiVersion("1.0")] + [Route("api/v{version:apiVersion}/...")], giving
  routes /api/v1/auth/..., /api/v1/permissions, /api/v1/roles/....

Reason:
URL-segment versioning makes the API version explicit and discoverable in
the route itself, which is the most common convention for REST APIs that
need to support breaking changes across major versions side by side. Because
the version segment is part of every route template, unversioned routes
(/api/auth/..., etc.) return 404, so every future endpoint is versioned from
the start.

---

# ADR-048

Decision:
Apply ASP.NET Core's built-in rate limiting middleware with two tiers: a
global IP-partitioned fixed-window policy applied to all endpoints, and
stricter named policies (login, register, refresh-token) applied to the
corresponding AuthController actions. Limits are bound from a RateLimiting
section in appsettings via IOptionsMonitor<RateLimitingOptions> and read per
request.

Status:
Approved

Implementation:

* Platform.API.RateLimiting.RateLimitingOptions / RateLimitPolicyOptions:
  Global, Login, Register, RefreshToken sections, each with PermitLimit and
  WindowSeconds; defaults (100/60, 5/60, 3/60, 10/60) match appsettings.json
  and apply if a section is absent.
* Platform.API.RateLimiting.RateLimitingPolicies: policy name constants
  (login, register, refresh-token).
* Platform.API.RateLimiting.RateLimitingServiceCollectionExtensions
  .AddRateLimitingPolicies:
  services.Configure<RateLimitingOptions>(configuration.GetSection(
      RateLimitingOptions.SectionName));
  AddRateLimiter with GlobalLimiter plus three named policies, all built via
  RateLimitPartition.GetFixedWindowLimiter partitioned by
  httpContext.Connection.RemoteIpAddress, with options resolved per request
  through IOptionsMonitor<RateLimitingOptions>.CurrentValue.
* AuthController: [EnableRateLimiting(RateLimitingPolicies.Register/Login/
  RefreshToken)] on Register/Login/RefreshToken actions.
* OnRejected returns 429 with a ProblemDetails body
  (application/problem+json) and a Retry-After header when available.
* Program.cs: app.UseRateLimiter() before authentication/authorization.

Reason:
A global baseline limit protects every endpoint by default, while
authentication endpoints - the most attractive targets for brute-force,
credential-stuffing, and account-enumeration attacks - get independently
configurable, stricter limits. IP-based partitioning is a reasonable default
without requiring an authenticated identity. Reading options through
IOptionsMonitor (rather than capturing a snapshot once at startup) keeps the
limiter responsive to configuration added after the initial AddApiServices
call - for example, test hosts overriding RateLimiting values via
ConfigureAppConfiguration - and matches the requirement that these values be
overridable per environment.
