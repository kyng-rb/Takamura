---
name: add-migration
description: Create an EF Core migration for Takamura.Application safely. Use after changing entities, DatabaseContext, or entity configurations.
---

# Add an EF Core migration

1. Confirm the entity/config change builds: `dotnet build src/Takamura.Application`.
2. The design-time factory (`Database/DatabaseContextFactory.cs`) reads env var `TakamuraConnectionString`. Check it is set (`[ -n "$TakamuraConnectionString" ]`) without printing it. If not set, ask the user to export it — never read `appsettings.Development.json`.
3. Generate: `dotnet ef migrations add <PascalCaseName> --project src/Takamura.Application`.
4. Review the generated `Up`/`Down`: watch for dropped columns/tables, `varchar(100)` truncation of long strings, and accidental tables for keyless types (e.g. `MonthlySubCategoryBalanceEntity`).
5. Produce a script for the user to review: `dotnet ef migrations script --idempotent --project src/Takamura.Application -o migration.sql` (do not commit the .sql unless asked).
6. **Do not run `dotnet ef database update`.** The connection string points at the live Azure SQL database. Tell the user the command and let them decide.
