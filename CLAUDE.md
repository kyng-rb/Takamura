# Takamura

Personal finance manager focused on **budgeting**. One user (the owner), amounts in USD, category names in Spanish (e.g. "Hipoteca", "Gasolina", "Ahorro").

Product vision: `README.md` (zero-based budgeting with rollover and sinking funds). Read it before changing how budgets, balances or allocations behave.

## Solution layout

```
src/
  Takamura.sln                 # API + Application + Web
  Takamura.API/                # ASP.NET Core 9 Minimal API (REST, OpenAPI + Scalar docs)
  Takamura.Application/        # Business logic + EF Core (SQL Server). Referenced by API.
  Takamura.Web/                # Blazor Web App (Interactive Server) + MudBlazor. Talks to the API over HTTP (RestSharp). No project reference to Application.
.github/workflows/             # Push to `develop` deploys the API and the Web app to Azure App Service
```

Stack: .NET 9, C# with `Nullable` + `ImplicitUsings`, EF Core 9 SqlServer, FluentResults, Humanizer, MudBlazor 8, RestSharp 113.

## Commands

```bash
dotnet build src/Takamura.sln                         # all three projects
dotnet run --project src/Takamura.API --launch-profile http   # http://localhost:5240  (docs: /scalar/v1, spec: /openapi/v1.json)
dotnet run --project src/Takamura.Web --launch-profile http   # http://localhost:5102  (start the API first)
```

EF Core migrations (design-time factory reads the env var `TakamuraConnectionString`):

```bash
export TakamuraConnectionString="..."   # never commit it
dotnet ef migrations add <Name> --project src/Takamura.Application
dotnet ef database update --project src/Takamura.Application   # ASK THE USER FIRST (unless it targets the local container below)
```

There is **no test project** yet. "Verify" currently means: both builds succeed with no new warnings, plus a smoke test against the local database below when behaviour changed.

### Local database (Docker)

A local SQL Server 2022 container is used for smoke tests and migrations, so nothing touches Azure:

```bash
colima start --vm-type vz --vz-rosetta              # Apple Silicon: SQL Server is x86-only and needs Rosetta
docker start takamura-sql                           # created once with mcr.microsoft.com/mssql/server:2022-latest on port 1433
set -a; . ./.env; set +a                            # .env (gitignored) holds the local SA password and both connection-string env vars
dotnet ef database update --project src/Takamura.Application   # applies migrations to the LOCAL container only
```

`.claude/launch.json` has `api-local` / `web-local` configurations that load `.env`, so the API runs against the container instead of the Azure database in `appsettings.Development.json`. The container has mock data only, and `dbo.monthly_sub_category_balance` there is a hand-written stand-in, not the production function (see backlog D1).

## Domain model (Takamura.Application/Database/Entities)

- `Category` 1-* `SubCategory` — the classification tree. Everything money-related points at a **SubCategory**.
- `Budget` — a named budget (`Title`). Owns allocations and bills.
- `PeriodAllocation` — planned money for a SubCategory in a month: `Amount`, `MonthFrom`, `YearFrom`, `Type` (`BudgetType`: Income | Outcome | Saving), `Description`.
- `Bill` — an actual expense: `Date` (DateOnly), `Amount`, `Description`, SubCategory, Budget.
- `MonthlySubCategoryBalance` — keyless read model mapped to the SQL table-valued function `dbo.monthly_sub_category_balance(budgetId, year, month?)` → (Budget, Period_Budget, Available_Amount, Month, Year, Category, Sub_Category). **The function's SQL lives only in the database; it is not in any migration.**

All entities inherit `EntityBase` (`Id`, `Status` Created/Updated/Deleted, `CreatedAt`, `LastUpdateAt`, all UTC). Entities are created through static `Create(...)` factories that set `Id = 0`, `CreatedAt = DateTime.UtcNow`, `Status = Created`.

DB conventions (DatabaseContext.ConfigureConventions): `decimal` → `decimal(18,2)`, `string` → `varchar(100)`, enums stored as strings `varchar(20)`, `DateTime` → `datetime`. Table names via `[Table("Singular")]`.

## Architecture: vertical slices

A feature is one slice across three layers. Names follow `<Verb><Thing>` with verbs `Create`, `Retrieve`, `Attach`.

**Application** — `Features/<Area>/<Action>/<Action>Service.cs`, one file holding:
- `record ...ServiceInput` / `...Input` / `...Options` (with `ToEntity()` when it creates something)
- `record ...Output` with static `FromEntity` / `FromEntities`
- `class ...Service(DatabaseContext context)` — primary constructor, `private readonly DatabaseContext _context = context;`, single `public async Task<Result>` or `Task<Result<TOutput>> Handle(input)`.
- Validation is inline at the top of `Handle`, returning `Result.Fail("message")` (first error becomes the HTTP 400 body). Check existence of referenced rows with `AnyAsync`. Reads use `.AsNoTracking()`. Use `.ConfigureAwait(false)` on awaits.
- Register in `Takamura.Application/DependencyInjection.cs` (`AddScoped`).

**API** — `Endpoints/<Group>/<Action>/<Action>Endpoint.cs` (+ `<Action>Request.cs` record with `ToServiceInput(routeId)`):
- `public static class XEndpoint { public static void Map(RouteGroupBuilder group) { group.MapGet/MapPost(..., async (...) => await service.Handle(...).ToHttp()).Produces<...>(); } }`
- Use `[FromRoute]`, `[FromBody]`, `[AsParameters]` (query), `[FromServices]`.
- `.ToHttp()` (Extensions/HTTPExtensions.cs): `Result` → 204 / 400, `Result<T>` → 200 / 400; `.ToCreated()` for create services (`Result<XOutput>` with `record XOutput(int Id)`) → 201 `{ "id": n }` / 400; the 400 body is a JSON string of all error messages. Binding failures and unhandled exceptions return problem details (`AddProblemDetails` + `UseExceptionHandler`).
- Wire it in the group file (`BudgetEndpointGroup` = `api/budget`, `CategoryEndpointGroup` = `api/category`). Bills and allocations are nested under a budget: `api/budget/{id}/bill`, `api/budget/{id}/allocation`.

**Web** — the client mirrors the API with its own DTOs (do not reference Application):
- `Services/<Area>/<Action>/<Area><Action>Service.cs`: input/output records + a class taking `IRestClientFactory`, `CreateClient(ApiSettings.ApiName)`, then `RestResponseGuard.EnsureSuccess(...)` or `EnsureSuccessAndData(...)`. Register in `Services/DependencyInjection.cs`.
- `Components/Pages/<Area>/<Action>/<Name>Page.razor` (routable) or `<Name>Dialog.razor` (MudDialog) + code-behind `.razor.cs` partial class.
- Errors: `RestResponseGuard` throws `ApiValidationException` for 400 (message = the API's text, read from a JSON string or problem details) and `ApiRequestException` otherwise. Dialogs pass `onValidationError: message => _errorMessage = message` to `ErrorRedirectExecutor.Run` and show `_errorMessage` in a `MudAlert` at the top of `DialogContent`; anything else goes to `/Error`.
- Page pattern: `_loading` flag, a `CancellationTokenSource _requestCts` cancelled in `Dispose()`, every call wrapped in `ErrorRedirectExecutor.Run(async () => ..., Nav, () => _loading = false)` (navigates to `/Error` on exceptions).
- Dialogs close with `MudDialog.Close(DialogResult.Ok(true))`; the caller reloads when `!result.Canceled`.
- Money is shown with `ToString("C1", CultureInfo.GetCultureInfo("en-US"))`; the UI allows one decimal place (`ValidateAmount`). Month names use `CultureInfo.InvariantCulture`.
- Both constructor injection and `[Inject]` properties are used; prefer whatever the neighbouring file uses.

Routes: `/` budgets, `/categories`, `/budgets/{id}/bills`, `/budgets/{id}/allocations`, `/budgets/{id}/balances`, `/Error`.

## Config & secrets

- API connection string key: `ConnectionStrings:TakamuraConnectionString`. Locally in `src/Takamura.API/appsettings.Development.json` (gitignored). **It points to the real Azure SQL database** — treat any write against it (migrations, test data) as production. Never print, copy, or commit it.
- Web needs `API:Host` (empty in appsettings.json; Azure sets `API__Host`). Locally it comes from `src/Takamura.Web/appsettings.Development.json` (gitignored) = `http://localhost:5240`.
- Deploys happen automatically on push to `develop`. Don't push to `develop`/`master` without the user asking.

## Known issues and backlog

Everything known but not yet scheduled lives in [docs/BACKLOG.md](docs/BACKLOG.md). Fix an entry only when asked, and delete it in the same PR. New findings go there too.

## Working agreements for Claude

- Follow the slice layout above exactly; put a new feature in all three layers when it is user-facing.
- After changes, run both builds. If `dotnet` isn't available, say so instead of claiming it compiles.
- Don't edit files under `Migrations/` by hand; generate them with `dotnet ef`. Never run `database update` against Azure without explicit approval; the local container is fine.
- Keep commits small, imperative, English ("Added X" is the existing style).
