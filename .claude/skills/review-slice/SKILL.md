---
name: review-slice
description: Review Takamura changes (current diff or a named feature) for correctness and consistency across Application, API and Web. Use before committing or when asked to review.
---

# Review a change

Scope: `git diff` (staged + unstaged) unless the user names files/feature.

Check:
- **Money correctness**: `decimal` everywhere (never double/float), amounts > 0 validated, sums/aggregations correct, month 1–12 and year ranges validated, DateOnly vs DateTime/UTC handling.
- **Contract drift**: API request/response record property names and types match the Web client DTOs and the query-parameter names the Web sends (see known issue #1 in CLAUDE.md).
- **Conventions**: slice layout, `Result.Fail` validation, `.ToHttp()`, DI registration in both DependencyInjection files, `ErrorRedirectExecutor` + `_requestCts` in pages.
- **EF**: `.AsNoTracking()` on reads, `Include` for navigation used in `FromEntity` (null-forgiving `!` hides missing includes), no N+1 loops, soft-deleted rows (`Status.Deleted`) handled if relevant.
- **Secrets**: no connection strings, keys or personal financial data committed.
- Build both projects and report results.

Report findings ordered by severity with file:line and a concrete fix. Don't change code unless asked.
