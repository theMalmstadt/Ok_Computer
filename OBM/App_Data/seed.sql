

INSERT INTO [dbo].Event (EventName, OrganizerID, [Public])VALUES
	('BigJims tournament', 'd93857cd-912d-435e-b530-474e15028fbc', 1);

INSERT INTO [dbo].Event (EventName, OrganizerID, [Public])VALUES
	('LittleJims tournament', 'd93857cd-912d-435e-b530-474e15028fbc', 1);

INSERT INTO [dbo].Event (EventName, OrganizerID, [Public],Location)VALUES
	('HUGEJims tournament','d93857cd-912d-435e-b530-474e15028fbc', 1, 97333);


	  
INSERT INTO [dbo].[Competitor] (CompetitorName, EventID)VALUES
	('MediumJim', 7);

INSERT INTO [dbo].[Tournament] (TournamentName, EventID, isTeams, UrlString)VALUES
	('MediumJims minecraft minetality', 11, 0, '123123123123');

	INSERT INTO [dbo].[Tournament] (TournamentName, EventID, isTeams, UrlString)VALUES
	('MediumJims minecraft minetality', 18, 0, '123123123123')
	INSERT INTO [dbo].[Tournament] (TournamentName, EventID, isTeams, UrlString)VALUES
	('MediumJims minecraft minetality', 12, 0, '123123123123');
	INSERT INTO [dbo].[Tournament] (TournamentName, EventID, isTeams, UrlString)VALUES
	('MediumJims minecraft minetality', 9, 0, '123123123123');