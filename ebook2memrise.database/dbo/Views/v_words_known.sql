CREATE VIEW [dbo].[v_words_known]
	AS SELECT * FROM dbo.words
	WHERE translation = ''
