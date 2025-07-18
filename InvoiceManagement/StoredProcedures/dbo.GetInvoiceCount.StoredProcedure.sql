USE [InvoiceManagement]
GO
/****** Object:  StoredProcedure [dbo].[GetInvoiceCount]    Script Date: 26-06-2025 19:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInvoiceCount]
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SELECT COUNT(*) 
    FROM InvoiceHeader
    WHERE (@SearchTerm IS NULL OR InvoiceNumber LIKE '%' + @SearchTerm + '%' OR Title LIKE '%' + @SearchTerm + '%')
END
GO
