DROP FUNCTION IF EXISTS dbo.monthly_sub_category_balance
DROP FUNCTION IF EXISTS dbo.monthly_category_balance
DROP FUNCTION IF EXISTS dbo.year_to_date_balance
GO
create function dbo.monthly_category_balance(@budget_id int, @year int, @month int = NULL)
returns @summary TABLE
(
    Budget VARCHAR(150),
    Period_Budget DECIMAL(18, 2),
    Available_Amount DECIMAL(18, 2),
    [Month] INT,
    [Year] INT,
    Category VARCHAR(150)
)
AS
BEGIN

    DECLARE @Budget VARCHAR(50) = (SELECT b.Title FROM dbo.Budget as b WHERE b.id = @budget_id)

    ;WITH allocation_summary
    AS (
        SELECT b.id AS budgetId
            , c.Id as CategoryId
            , sum(pa.Amount) AS amount
            , pa.MonthFrom AS [month]
            , pa.YearFrom AS [year]
        FROM dbo.Budget AS b
        INNER JOIN dbo.PeriodAllocation AS pa
            ON pa.BudgetId = b.Id
        INNER JOIN dbo.SubCategory as sc
            on sc.Id = pa.SubCategoryId
        INNER JOIN dbo.Category as c
            on c.id = sc.CategoryId
        WHERE
            b.Id = @budget_id
            AND pa.YearFrom = @year
            AND (@month is null OR pa.MonthFrom = @month)
        GROUP BY pa.MonthFrom
            , pa.YearFrom
            , c.Id
            , b.id
        )
        , bills_summary
    AS (
        SELECT b.id AS budgetId
            , c.Id as CategoryId
            , sum(bill.Amount) AS amount
            , year(bill.Date) AS [year]
            , month(bill.[Date]) AS [month]
        FROM dbo.Budget AS b
        INNER JOIN dbo.Bill AS bill
            ON bill.BudgetId = b.Id
        INNER JOIN dbo.SubCategory as sc
            on sc.Id = bill.SubCategoryId
        INNER JOIN dbo.Category as c
            on c.id = sc.CategoryId
        WHERE
            b.Id = @budget_id
            AND YEAR(bill.[Date]) = @year
            AND (@month is null OR MONTH(bill.[Date]) = @month)
        GROUP BY year(bill.Date)
            , month(bill.Date)
            , c.Id
            , b.id
        )
    , months
    AS (
        SELECT 1 as [month] UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
        UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8
        UNION SELECT 9 UNION SELECT 10 UNION SELECT 11 UNION SELECT 12
    )

    INSERT INTO @Summary
    SELECT @Budget AS budget
        , ISNULL(summary.amount, 0) AS period_budget
        , ISNULL(summary.amount, 0) - isnull(bills.amount, 0) AS available_amount
        , coalesce([month].[month], summary.month, bills.month) as [month]
        , @year as [year]
        , ISNULL(c.[Description], 'None') AS category
    FROM months as [month]
    LEFT JOIN allocation_summary AS summary
        ON summary.[month] = [month].[month]
    FULL JOIN bills_summary AS bills
        ON bills.budgetId = summary.budgetId
        AND bills.[year] = summary.[year]
        AND bills.[month] = summary.[month]
        AND bills.CategoryId = summary.CategoryId
    LEFT JOIN dbo.Category AS c
        ON c.Id = coalesce(summary.CategoryId, bills.CategoryId)
    WHERE
        @month IS NULL OR [month].[month] = @month

    RETURN
END
go

create function dbo.monthly_sub_category_balance(@budget_id int, @year int, @month int = NULL)
returns @summary TABLE
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

    DECLARE @Budget VARCHAR(50) = (
		SELECT b.Title
		FROM dbo.Budget AS b
		WHERE b.id = @budget_id
		);

    WITH allocation_summary
    AS (
        SELECT b.id AS budgetId
            , pa.SubCategoryId
            , sum(pa.Amount) AS amount
            , pa.MonthFrom AS [month]
            , pa.YearFrom AS [year]
        FROM dbo.Budget AS b
        INNER JOIN dbo.PeriodAllocation AS pa
            ON pa.BudgetId = b.Id
        WHERE b.Id = @budget_id
            AND pa.YearFrom = @year
            AND (
                @month IS NULL
                OR pa.MonthFrom = @month
                )
        GROUP BY pa.MonthFrom
            , pa.YearFrom
            , pa.SubCategoryId
            , b.id
        )
        , bills_summary
    AS (
        SELECT b.id AS budgetId
            , bill.SubCategoryId
            , sum(bill.Amount) AS amount
            , year(bill.DATE) AS [year]
            , month(bill.[Date]) AS [month]
        FROM dbo.Budget AS b
        INNER JOIN dbo.Bill AS bill
            ON bill.BudgetId = b.Id
        WHERE b.Id = @budget_id
            AND YEAR(bill.[Date]) = @year
            AND (
                @month IS NULL
                OR MONTH(bill.[Date]) = @month
                )
        GROUP BY year(bill.DATE)
            , month(bill.DATE)
            , bill.SubCategoryId
            , b.id
        )
    , months
    AS (
        SELECT 1 as [month] UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
        UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8
        UNION SELECT 9 UNION SELECT 10 UNION SELECT 11 UNION SELECT 12
    )

    INSERT INTO @Summary
    SELECT @Budget AS budget
        , ISNULL(summary.amount, 0) AS period_budget
        , ISNULL(summary.amount, 0) - isnull(bills.amount, 0) AS available_amount
        , coalesce([month].[month], summary.month, bills.month) as [month]
        , @year AS [year]
        , ISNULL(c.[Description], 'None') AS category
        , ISNULL(sc.[Description], 'None') AS sub_category
    FROM months AS [month]
    LEFT JOIN allocation_summary AS summary
        ON summary.[month] = [month].[month]
    FULL JOIN bills_summary AS bills
        ON bills.budgetId = summary.budgetId
        AND bills.[year] = summary.[year]
        AND bills.SubCategoryId = summary.SubCategoryId
    LEFT JOIN dbo.SubCategory AS sc
        ON sc.id = coalesce(summary.SubCategoryId, bills.SubCategoryId)
    inner JOIN dbo.Category AS c
        ON c.Id = sc.CategoryId
    WHERE
        @month IS NULL OR [month].[month] = @month

    RETURN
END

go
create function dbo.year_to_date_balance(@budget_id int)
returns @summary TABLE
(
    Budget VARCHAR(150),
    Period_Budget DECIMAL(18, 2),
    Available_Amount DECIMAL(18, 2),
    Category VARCHAR(150),
    Sub_Category VARCHAR(150)
)
AS
BEGIN

    declare @month int = MONTH(GETDATE())
    declare @year int = YEAR(GETDATE())

    DECLARE @Budget VARCHAR(50) = (
            SELECT b.Title
            FROM dbo.Budget AS b
            WHERE b.id = @budget_id
            );

    WITH allocation_summary
    AS (
        SELECT b.id AS budgetId
            , pa.SubCategoryId
            , sum(pa.Amount) AS amount
            , pa.YearFrom AS [year]
        FROM
            dbo.Budget AS b
        INNER JOIN
            dbo.PeriodAllocation AS pa
            ON pa.BudgetId = b.Id
        WHERE
            b.Id = @budget_id
            AND pa.YearFrom = @year
            AND pa.MonthFrom <= @month
        GROUP BY
            pa.YearFrom
            , pa.SubCategoryId
            , b.id
        )
        , bills_summary
    AS (
        SELECT
            b.id AS budgetId
            , bill.SubCategoryId
            , sum(bill.Amount) AS amount
            , year(bill.DATE) AS [year]
        FROM dbo.Budget AS b
        INNER JOIN dbo.Bill AS bill
            ON bill.BudgetId = b.Id
        WHERE
            b.Id = @budget_id
        GROUP BY year(bill.DATE)
            , bill.SubCategoryId
            , b.id
        )

    INSERT INTO @Summary
    SELECT @Budget AS budget
        , ISNULL(summary.amount, 0) AS period_budget
        , ISNULL(summary.amount, 0) - isnull(bills.amount, 0) AS available_amount
        , ISNULL(c.[Description], 'None') AS category
        , ISNULL(sc.[Description], 'None') AS sub_category
    FROM allocation_summary AS [summary]
    FULL JOIN bills_summary AS bills
        ON bills.budgetId = summary.budgetId
        AND bills.[year] = summary.[year]
        AND bills.SubCategoryId = summary.SubCategoryId
    LEFT JOIN dbo.SubCategory AS sc
        ON sc.id = coalesce(summary.SubCategoryId, bills.SubCategoryId)
    inner JOIN dbo.Category AS c
        ON c.Id = sc.CategoryId

    RETURN
END