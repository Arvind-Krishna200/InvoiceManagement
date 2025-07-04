USE [InvoiceManagement]
GO
/****** Object:  StoredProcedure [dbo].[GetInvoiceLineItemsByInvoiceId]    Script Date: 26-06-2025 19:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInvoiceLineItemsByInvoiceId]
    @InvoiceID INT
AS
BEGIN
    SELECT 
        LineItemID,
        InvoiceID,
        ItemCode,
        Description,
        UnitRate,
        Quantity
    FROM InvoiceLineItem
    WHERE InvoiceID = @InvoiceID
    ORDER BY LineItemID;
END
GO
