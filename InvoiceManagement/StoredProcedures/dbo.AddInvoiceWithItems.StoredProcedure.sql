USE [InvoiceManagement]
GO
/****** Object:  StoredProcedure [dbo].[AddInvoiceWithItems]    Script Date: 26-06-2025 19:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddInvoiceWithItems]
    @InvoiceNumber NVARCHAR(50),
    @Title NVARCHAR(100),
    @InvoiceDate DATE,
    @LineItems InvoiceLineItemType READONLY  -- We’ll define this table type
AS
BEGIN
    DECLARE @NewInvoiceID INT;

    INSERT INTO InvoiceHeader (InvoiceNumber, Title, InvoiceDate)
    VALUES (@InvoiceNumber, @Title, @InvoiceDate);

    SET @NewInvoiceID = SCOPE_IDENTITY();

    INSERT INTO InvoiceLineItem (InvoiceID, ItemCode, Description, UnitRate, Quantity)
    SELECT @NewInvoiceID, ItemCode, Description, UnitRate, Quantity
    FROM @LineItems;
END
GO

