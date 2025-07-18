USE [InvoiceManagement]
GO
/****** Object:  StoredProcedure [dbo].[GetInvoiceById]    Script Date: 26-06-2025 19:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInvoiceById]
    @InvoiceID INT
AS
BEGIN
    -- Get Invoice Header
    SELECT InvoiceID, InvoiceNumber, Title, InvoiceDate
    FROM InvoiceHeader
    WHERE InvoiceID = @InvoiceID;

    -- Get Invoice Line Items
    SELECT 
        LineItemID, InvoiceID, ItemCode, Description, UnitRate, Quantity,
        (UnitRate * Quantity) AS LineTotal
    FROM InvoiceLineItem
    WHERE InvoiceID = @InvoiceID;
END
GO
