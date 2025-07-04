USE [InvoiceManagement]
GO
/****** Object:  UserDefinedTableType [dbo].[InvoiceLineItemType]    Script Date: 26-06-2025 19:45:20 ******/
CREATE TYPE [dbo].[InvoiceLineItemType] AS TABLE(
	[LineItemID] [int] NULL,
	[ItemCode] [nvarchar](50) NULL,
	[Description] [nvarchar](255) NULL,
	[UnitRate] [decimal](18, 2) NULL,
	[Quantity] [int] NULL
)
GO
