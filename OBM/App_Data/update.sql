ALTER TABLE [dbo].[Tournament]
    ADD IsStarted BIT NOT NULL 
    CONSTRAINT df_IsStarted DEFAULT (0);