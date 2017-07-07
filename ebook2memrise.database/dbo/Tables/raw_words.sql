CREATE TABLE [dbo].[raw_words]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[word] VARCHAR(250)  NOT NULL,
	[translated] BIT NOT NULL DEFAUlt(0),
	[exported] BIT NOT NULL DEFAUlt(0)
)
