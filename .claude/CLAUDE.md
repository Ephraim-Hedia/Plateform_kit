# CLAUDE.md

## Project Overview

This project is not a business application.

The goal is to build a reusable backend platform that serves as the foundation for future .NET applications.

This platform must provide a production-ready implementation of the most common backend concerns so that future projects can focus primarily on business requirements rather than infrastructure.

The platform is intended to be copied and extended for future projects including but not limited to:

* E-Commerce Systems
* ERP Systems
* HR Systems
* Booking Platforms
* Inventory Systems
* Learning Platforms
* Internal Business Tools
* SaaS Products

The platform must remain domain-agnostic.

Business-specific functionality must never be added to reusable modules.

---

## Primary Objective

The objective of this project is to eliminate repetitive backend development work.

The platform should eventually provide reusable implementations for:

* Authentication
* Authorization
* Persistence
* Validation
* Logging
* Exception Handling
* API Infrastructure
* Auditing
* Notifications
* File Storage
* Caching
* Background Jobs

Future projects should be able to start from this platform and focus only on business requirements.

---

## Technology Stack

The following versions are fixed and must not be changed without explicit approval.

* .NET 9
* ASP.NET Core 9
* Entity Framework Core 9
* ASP.NET Identity
* MediatR 12
* FluentValidation 11
* AutoMapper
* Serilog
* SQL Server 2022
* Scalar
* xUnit
* NSubstitute
* FluentAssertions
* NetArchTest

---

## Solution Structure

Platform.sln

src/

* Platform.Domain
* Platform.Application
* Platform.Infrastructure
* Platform.API

tests/

* Platform.UnitTests
* Platform.IntegrationTests
* Platform.ArchitectureTests

Rules:

* All projects use the Platform namespace.
* Only src and tests folders exist at solution root.
* No business-specific projects are allowed in the platform.

---

## Architectural Style

The solution follows Clean Architecture.

Layers:

* Domain
* Application
* Infrastructure
* API

Dependency Direction:

API
↓
Infrastructure
↓
Application
↓
Domain

Rules:

* Domain must not depend on any other layer.
* Application may depend only on Domain.
* Infrastructure may depend on Domain and Application.
* API may depend on all layers.

Dependency rules are mandatory.

---

## Dependency Injection

Each layer owns its own service registration.

Required registration methods:

services.AddApplicationServices();

services.AddInfrastructureServices(configuration);

services.AddApiServices();

Rules:

* Program.cs should only orchestrate startup.
* Program.cs should not contain business registrations.
* Domain does not register services.
* Each layer is responsible for registering its own dependencies.

---

## Domain Layer Rules

The Domain layer represents the business core of the platform.

The Domain layer must remain framework-independent.

The following packages must never be referenced by Domain:

* Entity Framework Core
* ASP.NET Core
* MediatR
* Serilog
* JWT libraries
* Redis
* AutoMapper
* Any infrastructure package

Domain contains only:

* Entities
* Aggregate Roots
* Value Objects
* Domain Events
* Domain Errors
* Business Rules
* Domain Constants
* Result Pattern

Examples of forbidden concepts:

* DbContext
* Controller
* HttpContext
* ILogger
* JwtProvider
* IConfiguration

---

## Domain Foundation

The platform foundation includes:

* Entity<TId>
* AggregateRoot<TId>
* AuditableEntity<TId>
* ValueObject
* Error
* ErrorType
* Result
* Result<T>
* IDomainEvent
* ISoftDeletable

Rules:

* Entity<TId> is the base entity abstraction.
* AggregateRoot<TId> supports Domain Events.
* Domain Events are supported from day one.
* Publishing Domain Events is implemented later.
* AuditableEntity<TId> extends Entity<TId>.
* Soft delete is supported through ISoftDeletable.
* Use Guid as the default identifier type.
* Strongly typed IDs are not implemented in the platform foundation.

---

## Auditing

Auditing support exists from day one.

AuditableEntity<TId> contains:

* CreatedAt
* UpdatedAt
* CreatedBy
* UpdatedBy

Rules:

* Use DateTimeOffset.
* Store timestamps in UTC.
* Populate audit values automatically through EF Core interceptors.

---

## Soft Delete

Soft Delete support exists from day one.

ISoftDeletable contains:

* IsDeleted
* DeletedAt

Rules:

* Soft deleted entities must be filtered automatically through global query filters.
* Hard delete is not allowed for soft-deletable entities.
* IgnoreQueryFilters() must be used explicitly when deleted data is required.

---

## Result Pattern

Application use cases must use Result or Result<T>.

Expected business failures must not throw exceptions.

Examples:

* User not found
* Invalid credentials
* Permission denied
* Duplicate email

Expected failures should return Result.

Unexpected failures may throw exceptions.

Result contains a single Error.

Validation errors are handled by FluentValidation and pipeline behaviors.

---

## Error Design

Errors contain:

* Code
* Description
* ErrorType

Supported Error Types:

* Validation
* NotFound
* Conflict
* Unauthorized
* Forbidden
* Failure

API responses should map ErrorType values to HTTP status codes.

---

## CQRS

The platform follows CQRS.

Commands:

* Modify state
* Return Result or Result<T>
* Must not return entities

Queries:

* Read data
* Return DTOs
* Must not modify state

All handlers must accept CancellationToken.

All asynchronous operations must propagate CancellationToken.

This rule is mandatory.

---

## Feature Organization

Use feature-based organization.

Preferred:

Authentication/

* Login
* Register
* Logout
* RefreshToken

Permissions/

Roles/

Avoid:

* Commands
* Queries
* Validators

as top-level folders.

Everything related to a use case should live together.

---

## Authentication

Authentication is built on ASP.NET Identity.

Identity implementation lives in Infrastructure.

ApplicationUser and ApplicationRole belong to Infrastructure.

Identity responsibilities:

* User Management
* Password Hashing
* Password Reset
* Email Confirmation
* Account Lockout
* User Tokens
* External Authentication

The platform should not implement custom password hashing.

Always use ASP.NET Identity password hashing.

Passwords must never be:

* Logged
* Returned to clients
* Stored in plain text

---

## Authorization

Authorization is permission-based.

Architecture:

User
↓
Role
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

Preferred usage:

[HasPermission("Products.Create")]

Authorization must not hit the database on every request.

---

## Authorization Caching

Authorization uses IMemoryCache.

Cache:

* Roles
* Permissions
* RolePermission mappings

Do not cache:

* Passwords
* Refresh Tokens
* JWT Tokens

Cache must be invalidated whenever:

* A role changes
* A permission changes
* A role-permission assignment changes

Redis is not required in the platform foundation.

---

## JWT Strategy

JWT tokens contain:

* UserId
* Email
* Roles

Permissions are not stored in JWT.

Permission evaluation flow:

Request
↓
JWT
↓
Roles
↓
Memory Cache
↓
Permissions
↓
Authorization

This design allows immediate permission updates without waiting for token expiration.

---

## Refresh Tokens

Refresh Tokens are implemented as a custom entity.

Do not use Identity User Tokens for refresh token management.

RefreshToken must support:

* Revocation
* Expiration
* Multi-device login
* Rotation

Refresh Token Rotation is mandatory.

---

## Persistence

Use Entity Framework Core.

Requirements:

* Fluent Configurations
* SaveChanges Interceptors
* Audit Interceptors
* Global Query Filters
* Migrations

Avoid Generic Repository.

Prefer DbContext directly.

Use repositories only when they provide clear value.

---

## Current User Abstraction

Application must not access HttpContext directly.

Use:

ICurrentUser

Responsibilities:

* UserId
* Authentication state

Infrastructure is responsible for implementation using IHttpContextAccessor.

---

## Performance Rules

Read-only queries should use AsNoTracking().

Prefer projections over Includes.

Avoid N+1 query problems.

Do not optimize prematurely.

Prefer readability over micro-optimizations.

---

## Logging

Use Serilog.

Required sinks:

* Console
* File

Log:

* Requests
* Exceptions
* Authentication events
* Slow database queries

Never log:

* Passwords
* JWT Tokens
* Refresh Tokens
* Secrets

Use structured logging.

---

## API Rules

Use:

* Problem Details
* Scalar
* Health Checks
* API Versioning
* Rate Limiting

Controllers must remain thin.

Controllers:

* Receive requests
* Send Commands/Queries
* Return responses

Controllers must not contain business logic.

Controllers must not access DbContext directly.

Use MediatR for all use cases.

---

## Testing Strategy

Every module requires:

* Unit Tests
* Integration Tests
* Architecture Tests

Architecture Tests must verify:

* Domain → Infrastructure dependency forbidden
* Domain → API dependency forbidden
* Application → Infrastructure dependency forbidden
* Application → API dependency forbidden

Architecture violations must fail the build.

---

## Definition of Done

A module is complete only when:

* Domain implementation exists
* Application implementation exists
* Infrastructure implementation exists
* API endpoints exist
* Unit tests exist
* Integration tests exist
* Architecture tests pass
* Documentation is updated

Code generation alone does not mean completion.

---

## Decision Making

Before making architectural changes:

1. Explain trade-offs.
2. Explain alternatives.
3. Recommend a solution.
4. Wait for approval.

Major architectural decisions must never be made automatically.

---

## Long-Term Vision

The final result should become a reusable backend starter kit that can immediately provide:

* Authentication
* Authorization
* Persistence
* Validation
* Logging
* API Infrastructure

allowing future projects to focus almost entirely on business requirements.
