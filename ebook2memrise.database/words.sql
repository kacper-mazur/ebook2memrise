CREATE TABLE [dbo].[words]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[word] NVARCHAR(150) NOT NULL,
	[translation] NVARCHAR(150) NOT NULL,
	[definition] NVARCHAR(MAX) NULL,
	[example] NVARCHAR(MAX) NULL
)
GO

CREATE INDEX [ix_words]
ON [dbo].[words](word)
WITH fillfactor = 80
GO
