ALTER TABLE [dbo].[Tournament]
    ADD IsStarted BIT NOT NULL 
    CONSTRAINT df_IsStarted DEFAULT (0);

ALTER TABLE [dbo].[Match]
    ADD Score1 INT NULL; 

ALTER TABLE [dbo].[Match]
    ADD Score2 INT NULL; 

ALTER TABLE [dbo].[Match]
	ADD UpdatedAt DATETIME NOT NULL
	CONSTRAINT df_UpdatedAt DEFAULT (0);

ALTER TABLE [dbo].[Competitor]
	ADD PhoneNumber NVARCHAR(128) NULL;