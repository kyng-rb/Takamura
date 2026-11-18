# Backlog

Known problems and improvements that are **not scheduled yet**. They will be handled in a later refactor.
When you fix one, delete its entry in the same PR. When you find one, add it here instead of fixing it on the side.

Severity: **High** = data at risk or wrong numbers, **Medium** = wrong behaviour in an edge case, **Low** = quality/maintainability.

## Security

| # | Severity | Item |
|---|----------|------|
| S1 | High | **The API has no authentication or authorization.** It is deployed to Azure App Service on every push to `develop` and talks to the real database, so anyone who knows the URL can read and write all data. Needs a decision on the mechanism (e.g. Entra ID / App Service auth in front of both apps, or an API key shared between Web and API) before work starts. |
| S2 | Medium | Scalar (`/scalar/v1`) and the OpenAPI document (`/openapi/v1.json`) are mapped in every environment, production included. Decide together with S1 whether they stay public. |

## Known bugs

| # | Severity | Item |
|---|----------|------|
| B2 | Medium | Balances page: year-to-date always sums up to `DateTime.Today.Month`, even for past years (should be 12 for past years). |
| B3 | Low | `TryParseBudgetType` is duplicated in three Application services and parses with exceptions (Humanizer `DehumanizeTo`). |

## Data and persistence

| # | Severity | Item |
|---|----------|------|
| D1 | High | The table-valued function `dbo.monthly_sub_category_balance` (the rollover logic behind balances) lives only in the database. It is not in any migration, not reviewed and not tested. Version it (a migration with `migrationBuilder.Sql`) and cover it with tests. |
| D2 | Low | The keyless `MonthlySubCategoryBalanceEntity` made the Initial migration create an empty table `MonthlySubCategoryBalanceEntity`. Map it with `.ToView(null)` / as function-only and drop the table in a migration. |
| D3 | Low | No indexes besides the FK ones. Candidates: `Bill(BudgetId, Date)` and `PeriodAllocation(BudgetId, YearFrom, MonthFrom)`. |
| D4 | Low | Reads use `Include` and then map in memory. Project straight to the output records with `Select` so only the needed columns are read. |

## API

| # | Severity | Item |
|---|----------|------|
| A1 | Medium | No update or delete endpoints yet, although the entities already have `Update()`/`Delete()` soft-delete helpers. |
| A2 | Low | No GET-by-id endpoints, so `POST` cannot return a `Location` header. |
| A3 | Low | No health check endpoint (the root returns a plain string) and no request logging. |
| A4 | Low | OpenAPI metadata is thin: no `WithName`/`WithTags`/`WithSummary` on the endpoints. |

## Testing

| # | Severity | Item |
|---|----------|------|
| T1 | High | There is no test project. Start with unit tests for the service validation rules, then integration tests against SQL Server (a local container), including the balance function from D1. |

## Product gaps (compared with README.md)

These need a product decision before work starts.

| # | Item |
|---|------|
| P1 | There is no way to record income actually received; bills are expenses only. |
| P2 | Nothing computes "left to assign" (income minus assignments) for a month. |
| P3 | No "move money between subcategories" operation (README rule 2). |
| P4 | Sinking funds are not grouped apart from everyday spending. |
| P5 | `PeriodAllocation.MonthFrom`/`YearFrom` read like "effective from", but every query treats them as an exact month. Decide the meaning and rename or change the queries. |
