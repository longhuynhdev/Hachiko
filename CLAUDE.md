# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
This is "Hachiko", an ASP.NET Core 9.0 e-commerce web application built with the MVC pattern. The project implements a book store with categories and products, using Identity for authentication and Entity Framework Core with SQL Server for data persistence.

## Project Structure
The solution follows a clean architecture with separated concerns:

- **Hachiko** (main web project) - Controllers, Views, Program.cs, configuration
- **Hachiko.Models** - Domain models (Category, Product, ApplicationUser)
- **Hachiko.DataAccess** - Repository pattern implementation, DbContext, migrations
- **Hachiko.Utility** - Shared utilities and email services

## Database Setup
The project uses SQL Server with Entity Framework Core. Database migrations are stored in the main Hachiko project (not DataAccess project as of recent commits).

**Connection String**: Configured in `appsettings.json` to use SQL Server running on localhost with database "Hachiko"

**Docker SQL Server Setup** (from README):
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=SQLServer123@" \
-p 1433:1433 --name sqlserver -d \
mcr.microsoft.com/mssql/server:2022-latest
```

## Common Development Commands

### Build and Run
```bash
# Build the solution
dotnet build

# Run the application from Hachiko project directory
cd Hachiko
dotnet run

# Build entire solution from root
dotnet build EC-Project.sln
```

### Database Operations
```bash
# Apply migrations (run from main Hachiko project directory)
cd Hachiko
dotnet ef database update

# Create new migration (run from main Hachiko project directory)
dotnet ef migrations add MigrationName

# Install EF Core tools globally if not installed
dotnet tool install --global dotnet-ef
```

## Key Architecture Patterns

### Repository Pattern
The application uses the Unit of Work pattern with repositories:
- `IUnitOfWork` provides access to `ICategoryRepository` and `IProductRepository`
- Registered in DI container as scoped service
- Database context abstracted through repository layer

### Identity Integration
- Uses ASP.NET Core Identity with `IdentityUser` extended to `ApplicationUser`
- Email confirmation required for accounts
- Identity tables integrated with custom models via `ApplicationDbContext`

### Data Models
- **Category**: Basic category with Name, Description, DisplayOrder
- **Product**: Book entity with Title, Author, ISBN, multiple price tiers, foreign key to Category
- **ApplicationUser**: Extended IdentityUser for additional user properties

### MVC Structure
- Controllers use dependency injection for `IUnitOfWork`
- Views organized by controller (Category, Product, Home)
- Uses Razor Pages alongside MVC controllers
- Static files served from `wwwroot/images/product/` for product images