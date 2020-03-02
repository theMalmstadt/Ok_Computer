-- #######################################
-- #             Identity Tables         #
-- #######################################

-- ############# AspNetRoles #############
CREATE TABLE [dbo].[AspNetRoles]
(
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [dbo].[AspNetRoles]([Name] ASC);

-- ############# AspNetUsers #############
CREATE TABLE [dbo].[AspNetUsers]
(
    [Id]                   NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [ApiKey]               NVARCHAR (MAX) NULL,
    [Subdomain]            NVARCHAR(256)  NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [UserName]             NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([UserName] ASC);

-- ############# AspNetUserClaims #############
CREATE TABLE [dbo].[AspNetUserClaims]
(
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (128) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]([UserId] ASC);

-- ############# AspNetUserLogins #############
CREATE TABLE [dbo].[AspNetUserLogins]
(
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC),
    CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]([UserId] ASC);

-- ############# AspNetUserRoles #############
CREATE TABLE [dbo].[AspNetUserRoles]
(
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]([UserId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]([RoleId] ASC);


-- #######################################
-- #             Data Tables			 #
-- #######################################

-- ############# Event #############
CREATE TABLE [dbo].[Event](
	[EventID] INT IDENTITY (1,1) PRIMARY KEY,
	[OrganizerID] NVARCHAR(128) NOT NULL,
	[EventName] NVARCHAR(128) NOT NULL,
	[Description] NVARCHAR(256) NULL,
	[Location] NVARCHAR(256) NULL,
	[Public] BIT NOT NULL,
	CONSTRAINT [FK_dbo.Event_dbo.AspNetUsers_Id] Foreign KEY ([OrganizerID]) REFERENCES [dbo].[AspNetUsers] ([ID])
);
GO

-- ############# Tournament #############
CREATE TABLE [dbo].[Tournament](
    [TournamentID] INT IDENTITY (1,1) PRIMARY KEY,
    [TournamentName] NVARCHAR(128) NOT NULL,
    [EventID] INT NOT NULL,
    [Description] NVARCHAR(256) NULL,
    [Game] NVARCHAR(256) NULL,
    [ApiId] INT NULL,
    [UrlString] NVARCHAR(256) NOT NULL,
    [IsTeams] BIT NOT NULL,
    [IsStarted] BIT NOT NULL,
    CONSTRAINT [FK_dbo.Tournament_dbo.Event_EventID] Foreign KEY ([EventID]) REFERENCES [dbo].[Event] ([EventID])
);
GO

-- ############# Station #############
Create Table [dbo].[Station](
    [StationID] INT IDENTITY (1,1) PRIMARY KEY,
    [StationName] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(256) NULL
);
GO

-- ############# tournament_station #############
Create Table [dbo].[tournament_station](
    [ID] INT IDENTITY (1,1) PRIMARY KEY,
    [TournamentID] INT NOT NULL,
    [StationID] INT NOT NULL,
    CONSTRAINT [FK_dbo.tournament_station_dbo.Tournament_TournamentID] Foreign KEY ([TournamentID]) REFERENCES [dbo].[Tournament] ([TournamentID]),
    CONSTRAINT [FK_dbo.tournament_station_dbo.Station_StationID] Foreign KEY ([StationID]) REFERENCES [dbo].[Station] ([StationID])
);
GO

-- ############# Competitor #############
CREATE TABLE [dbo].[Competitor](
    [CompetitorID] INT IDENTITY (1,1) PRIMARY KEY,
    [CompetitorName] NVARCHAR (128) NOT NULL,
    [EventID] INT NOT NULL,
    [BusyState] NVARCHAR(1) NULL,
    CONSTRAINT [FK_dbo.Competitor_dbo.Event_EventID] Foreign KEY ([EventID]) REFERENCES [dbo].[Event] ([EventID])
);
GO

-- ############# Match #############
CREATE TABLE [dbo].[Match](
    [MatchID] INT IDENTITY (1,1) PRIMARY KEY,
    [TournamentID] INT NOT NULL,
    [Competitor1ID] INT NULL,
    [Competitor2ID] INT NULL,
    [Identifier] NVARCHAR(16),
    [Round] INT,
    [ApiID] INT NOT NULL,
    [PrereqMatch1ID] INT NULL,
    [PrereqMatch2ID] INT NULL,
    [Time] DATETIME NULL
    CONSTRAINT [FK_dbo.Match_dbo.Tournament_TournamentID] Foreign KEY ([TournamentID]) REFERENCES [dbo].[Tournament] ([TournamentID]),
    CONSTRAINT [FK_dbo.Match_dbo.Competitor_Competitor1ID] Foreign KEY ([Competitor1ID]) REFERENCES [dbo].[Competitor] ([CompetitorID]),
    CONSTRAINT [FK_dbo.Match_dbo.Competitor_Competitor2ID] Foreign KEY ([Competitor2ID]) REFERENCES [dbo].[Competitor] ([CompetitorID]),
    CONSTRAINT [FK_dbo.Match_dbo.Match_PrereqMatch1ID] Foreign KEY ([PrereqMatch1ID]) REFERENCES [dbo].[Match] ([MatchID]),
    CONSTRAINT [FK_dbo.Match_dbo.Match_PrereqMatch2ID] Foreign KEY ([PrereqMatch2ID]) REFERENCES [dbo].[Match] ([MatchID])
);
GO
