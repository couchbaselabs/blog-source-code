CREATE TABLE [dbo].[TicketCheck](
	[Id] [uniqueidentifier] NOT NULL,
	[FullName] [varchar](100) NOT NULL,
	[Section] [varchar](10) NOT NULL,
	[Row] [varchar](10) NOT NULL,
	[Seat] [varchar](10) NOT NULL,
	[GameDay] [datetime] NOT NULL,
 CONSTRAINT [PK_TicketCheck] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TicketCheck] ADD  CONSTRAINT [DF_TicketCheck_Id]  DEFAULT (newid()) FOR [Id]
GO


