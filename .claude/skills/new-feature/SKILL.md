---
name: new-feature
description: Scaffold a new Takamura vertical slice (Application service + API endpoint + Web client service/page) following the project's conventions. Use when adding a new endpoint, screen, or budget/category/bill/allocation capability.
---

# New vertical slice

Ask (or infer from the request): Area (Budget, Bill, Category, Allocation, Balance…), Action (`Create`, `Retrieve`, `Attach`, `Update`, `Delete`), inputs, outputs, and whether a UI is needed.

Before writing, open the closest existing slice and copy its shape:
- Create → `Features/Bill/Create/CreateBillService.cs`, `Endpoints/Budget/CreateBill/*`, `Services/Bills/Create/BillsCreateService.cs`, `Components/Pages/Bills/Create/*`
- Retrieve with filters → `Features/Bill/Retrieve/RetrieveBillsService.cs`, `Endpoints/Budget/RetrieveBills/*`, `Components/Pages/Bills/Retrieve/*`

Steps:
1. **Application** `src/Takamura.Application/Features/<Area>/<Action>/<Action><Thing>Service.cs` — input record, output record(s) with `FromEntity/FromEntities`, service with primary-ctor `DatabaseContext`, `Handle` returning `Result`/`Result<T>`, inline validation via `Result.Fail`, existence checks with `AnyAsync`, `.AsNoTracking()` on reads, `.ConfigureAwait(false)`.
2. If the schema changes: update the entity (keep the static `Create` factory pattern) then use the `add-migration` skill.
3. Register the service in `src/Takamura.Application/DependencyInjection.cs`.
4. **API** `src/Takamura.API/Endpoints/<Group>/<Action><Thing>/` — `*Request.cs` record with `ToServiceInput(id)` when route/query params are involved, `*Endpoint.cs` static `Map(RouteGroupBuilder)` using `.ToHttp()` and `.Produces<...>()`. Add the `Map` call in the group file. Nest budget-owned resources under `/{id:int}/...`.
5. **Web** (if UI) — client service in `src/Takamura.Web/Services/<Area>/<Action>/` (own DTOs matching the API JSON, `RestResponseGuard`), register in `Services/DependencyInjection.cs`; page/dialog + `.razor.cs` using `_loading`, `_requestCts`, `ErrorRedirectExecutor.Run`, USD `C1` formatting. Link it from an existing page or `MainLayout`.
6. Build: `dotnet build src/Takamura.sln` and `dotnet build src/Takamura.Web/Takamura.Web.csproj`. Fix all new warnings.
7. Summarise the new route(s) and files touched.
