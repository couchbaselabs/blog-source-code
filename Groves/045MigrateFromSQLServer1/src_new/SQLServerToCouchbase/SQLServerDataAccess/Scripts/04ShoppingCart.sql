CREATE TABLE [dbo].[ShoppingCart](
	[Id] [uniqueidentifier] NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	CONSTRAINT [PK_ShoppingCart] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ShoppingCart] ADD  CONSTRAINT [DF_ShoppingCart_Id]  DEFAULT (newid()) FOR [Id]
GO