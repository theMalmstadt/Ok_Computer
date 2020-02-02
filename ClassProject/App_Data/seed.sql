INSERT INTO [dbo].[Teams] VALUES
('Bobcats'),
('Lumberjacks');


INSERT INTO [dbo].[Athletes] VALUES
('Ethan Black'),
('Jacob Malmastadt'),
('Ashlyn Santiago'),
('Zak Keipp');

INSERT INTO [dbo].[team_athlete] VALUES
(1, 1),
(1, 2),
(2, 3),
(2, 4);


INSERT INTO [dbo].[Events] VALUES
('100'),
('50');


INSERT INTO [dbo].[Event_Results] VALUES
(1, 1, 12.001, '2020-01-31 22:39:04.107'),
(1, 1, 11.1, '2020-01-31 22:39:04.107'),
(2, 1, 12.3, '2020-01-31 22:39:04.107'),
(2, 1, 10.999, '2020-01-31 22:39:04.107'),
(3, 1, 15.15, '2020-01-31 22:39:04.107'),
(3, 2, 11.111, '2020-01-31 22:39:04.107'),
(4, 2, 10.101, '2020-01-31 22:39:04.107'),
(4, 2, 12.001, '2020-01-31 22:39:04.107'),
(1, 2, 6, '2020-01-31 22:39:04.107');