USE [InvoiceManagement]
GO
/****** Object:  UserDefinedTableType [dbo].[DeletedLineItemType]    Script Date: 26-06-2025 19:45:20 ******/
CREATE TYPE [dbo].[DeletedLineItemType] AS TABLE(
	[LineItemID] [int] NULL
)
GO
