USE [InvoiceManagement]
GO
/****** Object:  StoredProcedure [dbo].[UpdateInvoiceWithItems]    Script Date: 26-06-2025 19:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateInvoiceWithItems]
    @InvoiceID INT,
    @InvoiceNumber NVARCHAR(50),
    @Title NVARCHAR(100),
    @InvoiceDate DATE,
    @LineItems InvoiceLineItemType READONLY,
    @DeletedLineItems DeletedLineItemType READONLY  
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Update header
    UPDATE InvoiceHeader
    SET InvoiceNumber = @InvoiceNumber,
        Title = @Title,
        InvoiceDate = @InvoiceDate
    WHERE InvoiceID = @InvoiceID;

    -- 2. Merge for update + insert
    MERGE InvoiceLineItem AS target
    USING (
        SELECT * FROM @LineItems
    ) AS source
    ON target.LineItemID = source.LineItemID AND target.InvoiceID = @InvoiceID

    WHEN MATCHED THEN
        UPDATE SET
            ItemCode = source.ItemCode,
            Description = source.Description,
            UnitRate = source.UnitRate,
            Quantity = source.Quantity

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (InvoiceID, ItemCode, Description, UnitRate, Quantity)
        VALUES (@InvoiceID, source.ItemCode, source.Description, source.UnitRate, source.Quantity);

    -- 3. delete
    DELETE FROM InvoiceLineItem
    WHERE InvoiceID = @InvoiceID
    AND LineItemID IN (SELECT LineItemID FROM @DeletedLineItems);
END;
GO
