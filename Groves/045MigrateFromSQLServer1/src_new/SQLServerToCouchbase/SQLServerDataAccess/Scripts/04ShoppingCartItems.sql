CREATE TABLE [dbo].[ShoppingCartItems](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Quantity] [int] NOT NULL,
	[ShoppingCartId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_ShoppingCartItems] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ShoppingCartItems] ADD  CONSTRAINT [DF_ShoppingCartItems_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ShoppingCartItems]  WITH CHECK ADD  CONSTRAINT [FK_ShoppingCartItems_ShoppingCart] FOREIGN KEY([ShoppingCartId])
REFERENCES [dbo].[ShoppingCart] ([Id])
GO

ALTER TABLE [dbo].[ShoppingCartItems] CHECK CONSTRAINT [FK_ShoppingCartItems_ShoppingCart]
GO

