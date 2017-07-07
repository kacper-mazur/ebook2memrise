CREATE TABLE [dbo].[file_raw]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[file_name] VARCHAR(200) NOT NULL,
	[file_content] VARBINARY(MAX)
)
