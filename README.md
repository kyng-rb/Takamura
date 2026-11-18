# Takamura

Internal guide to what Takamura is, what it does, and where it stops. Check a change against this page before building it.

## Purpose

A personal budgeting tool that gives every dollar a job before the month starts and carries each month's results into the next, so the plan always matches real life.

## Core rules

1. **Zero-based:** income minus assignments equals zero. Savings, debt payments and future costs count as assignments.
2. **If it is not assigned, it is not spent.** When plans change, move money between categories. Never spend unassigned money.
3. **Rollover:** each category's balance carries into the next month.

   ```
   Available at month end = carried in + assigned this month − spent this month
   ```

   Unspent money stays with its purpose. Overspending lowers next month's balance until it is covered.
4. **Sinking funds:** an annual or occasional cost is budgeted monthly (yearly amount ÷ 12, or remaining amount ÷ months left) and rolls over until it is paid. These funds are grouped apart from everyday spending.

## In scope

- One owner, one currency (USD).
- Categories and subcategories. Every amount belongs to a subcategory.
- Budgets with monthly allocations typed as Income, Outcome or Saving.
- Bills recorded against a subcategory and a budget.
- Balances per subcategory and month, including rollover and year-to-date.

## Out of scope

- Multiple users, shared budgets or roles.
- Multiple currencies or exchange rates.
- Investments, net worth or financial advice.
- Bank connections or automatic transaction import. Data is entered by hand.
- Moving real money. Takamura plans and records; it never pays.

Anything outside these lines needs an explicit decision before work starts.

## Success looks like

- "Left to assign" is zero at the start of every month.
- Every category's available amount is clear and accurate at any time.
- Irregular costs are paid from the funds built for them.
- Month-to-month changes reflect real spending, not payment timing.

## For developers

Setup, architecture and conventions live in [CLAUDE.md](CLAUDE.md).
