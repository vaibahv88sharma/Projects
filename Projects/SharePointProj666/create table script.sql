USE [AAES Home]
GO

/****** Object:  Table [dbo].[TopViewedDocsTable11]    Script Date: 9/18/2015 6:33:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TopViewedDocsTable11](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DocName] [varchar](255) NULL,
	[DocLocation] [varchar](255) NULL,
	[DownloadCount] [int] NULL,
	[DataAddedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


