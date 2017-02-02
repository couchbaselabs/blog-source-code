CREATE TABLE [dbo].[FriendBookUsersFriends](
	[UserId] [uniqueidentifier] NOT NULL,
	[FriendUserId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_FriendBookUsersFriends] PRIMARY KEY CLUSTERED 
	(
		[UserId] ASC,
		[FriendUserId] ASC
	)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FriendBookUsersFriends]  WITH CHECK ADD  CONSTRAINT [FK_FriendBookUsersFriends_FriendBookUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[FriendBookUsers] ([Id])
GO
ALTER TABLE [dbo].[FriendBookUsersFriends] CHECK CONSTRAINT [FK_FriendBookUsersFriends_FriendBookUsers]
GO
ALTER TABLE [dbo].[FriendBookUsersFriends]  WITH CHECK ADD  CONSTRAINT [FK_FriendBookUsersFriends_FriendBookUsers1] FOREIGN KEY([UserId])
REFERENCES [dbo].[FriendBookUsers] ([Id])
GO
ALTER TABLE [dbo].[FriendBookUsersFriends] CHECK CONSTRAINT [FK_FriendBookUsersFriends_FriendBookUsers1]
GO