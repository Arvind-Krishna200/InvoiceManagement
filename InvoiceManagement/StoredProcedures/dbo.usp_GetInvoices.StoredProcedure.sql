USE [InvoiceManagement]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetInvoices]    Script Date: 26-06-2025 19:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[usp_GetInvoices]
    @SearchTerm NVARCHAR(100),
    @PageNumber INT,
    @PageSize INT,
    @SortColumn NVARCHAR(50),
    @SortDirection NVARCHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Sql NVARCHAR(MAX)

    SET @Sql = '
    SELECT ih.InvoiceID, ih.InvoiceNumber, ih.Title, ih.InvoiceDate,
           SUM(il.UnitRate * il.Quantity) AS TotalAmount
    FROM InvoiceHeader ih
    LEFT JOIN InvoiceLineItem il ON ih.InvoiceID = il.InvoiceID
    WHERE (@SearchTerm IS NULL OR @SearchTerm = '''' OR
           ih.InvoiceNumber LIKE ''%'' + @SearchTerm + ''%'' OR
           ih.Title LIKE ''%'' + @SearchTerm + ''%'')
    GROUP BY ih.InvoiceID, ih.InvoiceNumber, ih.Title, ih.InvoiceDate
    ORDER BY ' + QUOTENAME(@SortColumn) + ' ' + @SortDirection + '
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    '

    EXEC sp_executesql @Sql,
        N'@SearchTerm NVARCHAR(100), @PageNumber INT, @PageSize INT',
        @SearchTerm, @PageNumber, @PageSize;
END;
GO
