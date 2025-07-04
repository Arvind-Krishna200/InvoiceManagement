USE [InvoiceManagement]
GO
/****** Object:  Table [dbo].[InvoiceLineItem]    Script Date: 26-06-2025 19:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceLineItem](
	[LineItemID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NOT NULL,
	[ItemCode] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[UnitRate] [decimal](10, 2) NOT NULL,
	[Quantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[LineItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InvoiceLineItem]  WITH CHECK ADD FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[InvoiceHeader] ([InvoiceID])
GO
