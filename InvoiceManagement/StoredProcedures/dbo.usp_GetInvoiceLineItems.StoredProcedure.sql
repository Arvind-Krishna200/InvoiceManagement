USE [InvoiceManagement]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetInvoiceLineItems]    Script Date: 26-06-2025 19:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetInvoiceLineItems]
    @InvoiceID INT,
    @SearchTerm NVARCHAR(100),
    @PageNumber INT,
    @PageSize INT,
    @SortColumn NVARCHAR(50) = 'ItemCode',
    @SortDirection NVARCHAR(4) = 'ASC'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Sql NVARCHAR(MAX);
    DECLARE @OrderBy NVARCHAR(100);

    -- Validate @SortDirection to prevent SQL injection
    IF UPPER(@SortDirection) NOT IN ('ASC', 'DESC')
        SET @SortDirection = 'ASC';

    -- Prepare dynamic ORDER BY expression safely
    SET @OrderBy = 
        CASE 
            WHEN @SortColumn = 'LineTotal' THEN 'UnitRate * Quantity'
            WHEN @SortColumn IN ('ItemCode', 'Description', 'UnitRate', 'Quantity') THEN QUOTENAME(@SortColumn)
            ELSE 'ItemCode' -- fallback to default
        END + ' ' + @SortDirection;

    -- Build final SQL
    SET @Sql = '
    SELECT
        ItemCode,
        Description,
        UnitRate,
        Quantity,
        (UnitRate * Quantity) AS LineTotal
    FROM InvoiceLineItem
    WHERE InvoiceID = @InvoiceID
      AND (@SearchTerm IS NULL OR @SearchTerm = ''''
           OR ItemCode LIKE ''%'' + @SearchTerm + ''%''
           OR Description LIKE ''%'' + @SearchTerm + ''%'')
    ORDER BY ' + @OrderBy + '
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;';

    EXEC sp_executesql 
        @Sql,
        N'@InvoiceID INT, @SearchTerm NVARCHAR(100), @PageNumber INT, @PageSize INT',
        @InvoiceID, @SearchTerm, @PageNumber, @PageSize;
END;
GO
