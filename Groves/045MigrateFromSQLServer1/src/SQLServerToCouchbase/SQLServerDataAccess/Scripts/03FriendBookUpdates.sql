CREATE TABLE [dbo].[FriendBookUpdates](
	[Id] [uniqueidentifier] NOT NULL,
	[PostedDate] [datetime] NOT NULL,
	[Body] [nvarchar](256) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_FriendBookUpdates] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FriendBookUpdates]  WITH CHECK ADD  CONSTRAINT [FK_FriendBookUpdates_FriendBookUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[FriendBookUsers] ([Id])
GO
ALTER TABLE [dbo].[FriendBookUpdates] CHECK CONSTRAINT [FK_FriendBookUpdates_FriendBookUsers]
GO