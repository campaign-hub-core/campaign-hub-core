# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Campaign Hub is a campaign management system built with .NET 10.0 using Clean Architecture principles. The solution manages organizations, users, clients, advertising accounts, campaigns, and their performance metrics across multiple ad platforms (Meta Ads and Google Ads).

## Architecture

The solution follows **Clean Architecture** with clear layer separation:

- **CampaignHub.Domain**: Core business logic and domain entities (no dependencies)
- **CampaignHub.Application**: Application services and use cases (currently empty, planned for future implementation)
- **CampaignHub.Infra**: Data access layer with EF Core and PostgreSQL
- **CampaignHub.API**: ASP.NET Core Web API presentation layer

**Dependency Flow**: API → Application → Domain ← Infrastructure

## Core Domain Model

The system is built around these key entities:

1. **Organization** - Campaign management organizations
2. **User** - System users with email and password hash
3. **Client** - Customer accounts with types (Lead, Ecommerce, Institutional) and statuses (Inactive, Active, Suspended, Pending)
4. **AdAccount** - Advertising accounts linked to clients, supporting Meta Ads and Google Ads platforms
5. **Campaign** - Marketing campaigns with date ranges and statuses (Active, Paused, Completed, Cancelled)
6. **MetricCampaign** - Monthly performance metrics (expenses, leads, sales, revenue)

All entities inherit from `Entity` base class which provides:
- Auto-generated GUID-based `Id`
- `CreatedAt` timestamp (UTC)

Domain entities contain business logic methods (e.g., `Campaign.Pause()`, `Client.Enable()`, `MetricCampaign.UpdateExpenses()`).

## Database Configuration

**Provider**: PostgreSQL via Npgsql.EntityFrameworkCore.PostgreSQL v10.0.0

**DbContext**: `AppDbContext` in `CampaignHub.Infra/AppDbContext.cs`

**Entity Mappings**: FluentAPI configurations in `CampaignHub.Infra/Mapping/` directory using `IEntityTypeConfiguration<T>` pattern. Configurations are auto-discovered via assembly scanning.

**Important**: There is a typo in the filename `CampaingMapping.cs` (should be `CampaignMapping.cs`).

## Common Development Commands

### Build and Run

```bash
# Restore dependencies
dotnet restore

# Build entire solution
dotnet build

# Run API (from solution root)
dotnet run --project CampaignHub.API

# Run with HTTPS profile
dotnet run --project CampaignHub.API --launch-profile https
```

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project CampaignHub.Infra --startup-project CampaignHub.API

# Update database to latest migration
dotnet ef database update --project CampaignHub.Infra --startup-project CampaignHub.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project CampaignHub.Infra --startup-project CampaignHub.API

# Generate SQL script for migration
dotnet ef migrations script --project CampaignHub.Infra --startup-project CampaignHub.API
```

### API Endpoints

- **HTTP**: http://localhost:5239
- **HTTPS**: https://localhost:7224
- **OpenAPI**: Available in development environment at `/openapi/v1.json`

## Project Status

This is an early-stage project with foundational architecture in place. The following are **not yet implemented**:

- API controllers and endpoints (beyond placeholder)
- Application layer services/handlers
- Dependency injection registration for DbContext and services
- Repository pattern or CQRS implementation
- Authentication and authorization logic
- Database migrations
- Connection string configuration
- Unit and integration tests
- Error handling and validation
- Logging infrastructure

## Key Technical Decisions

1. **Rich Domain Models**: Entities contain business logic methods, not just data properties
2. **Enum Storage**: Enums are stored as integers in the database
3. **FluentAPI Mappings**: Prefer FluentAPI over data annotations for entity configuration
4. **Nullable Reference Types**: Enabled across all projects for improved type safety
5. **Implicit Usings**: Enabled to reduce boilerplate
6. **Monthly Metrics**: `MetricCampaign.ReferencePeriod` is automatically normalized to the first day of the month

## When Adding Features

1. Start with domain entities in `CampaignHub.Domain/Entities/`
2. Add FluentAPI mapping in `CampaignHub.Infra/Mapping/`
3. Register new DbSet in `AppDbContext.cs`
4. Create and apply EF Core migration
5. Implement application services in `CampaignHub.Application` (when layer is activated)
6. Expose via API controllers in `CampaignHub.API`

## Solution File Format

Uses `.slnx` format (XML-based modern Visual Studio solution format) instead of traditional `.sln`.
