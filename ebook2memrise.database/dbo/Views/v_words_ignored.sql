CREATE VIEW [dbo].[v_words_ignored]
	AS SELECT * FROM dbo.words
	WHERE translation = 'IGNORED'
