DROP FUNCTION IF EXISTS dbo.monthly_sub_category_balance;
GO

CREATE FUNCTION dbo.monthly_sub_category_balance
(
    @budget_id INT,
    @year INT,
    @month INT = NULL
)
RETURNS @summary TABLE
(
    Budget VARCHAR(150),
    Period_Budget DECIMAL(18, 2),
    Available_Amount DECIMAL(18, 2),
    [Month] INT,
    [Year] INT,
    Category VARCHAR(150),
    Sub_Category VARCHAR(150)
)
AS
BEGIN
    DECLARE @Budget VARCHAR(150) =
    (
        SELECT b.Title
        FROM dbo.Budget AS b
        WHERE b.Id = @budget_id
    );

    ;WITH allocation_summary AS
    (
        SELECT
            b.Id AS budgetId,
            pa.SubCategoryId,
            SUM(pa.Amount) AS amount,
            pa.MonthFrom AS [month],
            pa.YearFrom AS [year]
        FROM dbo.Budget AS b
        INNER JOIN dbo.PeriodAllocation AS pa
            ON pa.BudgetId = b.Id
        WHERE
            b.Id = @budget_id
            AND pa.YearFrom = @year
            AND (@month IS NULL OR pa.MonthFrom = @month)
        GROUP BY
            b.Id,
            pa.SubCategoryId,
            pa.MonthFrom,
            pa.YearFrom
    ),
    bills_summary AS
    (
        SELECT
            b.Id AS budgetId,
            bill.SubCategoryId,
            SUM(bill.Amount) AS amount,
            YEAR(bill.[Date]) AS [year],
            MONTH(bill.[Date]) AS [month]
        FROM dbo.Budget AS b
        INNER JOIN dbo.Bill AS bill
            ON bill.BudgetId = b.Id
        WHERE
            b.Id = @budget_id
            AND YEAR(bill.[Date]) = @year
            AND (@month IS NULL OR MONTH(bill.[Date]) = @month)
        GROUP BY
            b.Id,
            bill.SubCategoryId,
            YEAR(bill.[Date]),
            MONTH(bill.[Date])
    ),
    combined AS
    (
        SELECT
            COALESCE(a.budgetId, bl.budgetId) AS budgetId,
            COALESCE(a.SubCategoryId, bl.SubCategoryId) AS SubCategoryId,
            COALESCE(a.[year], bl.[year]) AS [year],
            COALESCE(a.[month], bl.[month]) AS [month],
            ISNULL(a.amount, 0) AS period_amount,
            ISNULL(bl.amount, 0) AS bills_amount
        FROM allocation_summary AS a
        FULL JOIN bills_summary AS bl
            ON bl.budgetId = a.budgetId
            AND bl.SubCategoryId = a.SubCategoryId
            AND bl.[year] = a.[year]
            AND bl.[month] = a.[month]
    )
    INSERT INTO @summary
    SELECT
        @Budget AS Budget,
        cmb.period_amount AS Period_Budget,
        cmb.period_amount - cmb.bills_amount AS Available_Amount,
        cmb.[month] AS [Month],
        cmb.[year] AS [Year],
        ISNULL(c.[Description], 'None') AS Category,
        ISNULL(sc.[Description], 'None') AS Sub_Category
    FROM combined AS cmb
    LEFT JOIN dbo.SubCategory AS sc
        ON sc.Id = cmb.SubCategoryId
    LEFT JOIN dbo.Category AS c
        ON c.Id = sc.CategoryId
    WHERE
        (@month IS NULL OR cmb.[month] = @month);

    RETURN;
END;
GO
