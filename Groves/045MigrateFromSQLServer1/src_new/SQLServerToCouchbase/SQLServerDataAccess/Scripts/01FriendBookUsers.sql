CREATE TABLE [dbo].[FriendBookUsers](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_FriendBookUsers] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FriendBookUsers] ADD  CONSTRAINT [DF_FriendBookUser_Id]  DEFAULT (newid()) FOR [Id]
GO