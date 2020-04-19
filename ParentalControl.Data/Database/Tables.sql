DROP TABLE IF EXISTS [dbo].[WebLimitations];
DROP TABLE IF EXISTS [dbo].[Keywords];
DROP TABLE IF EXISTS [dbo].[ProgramLimitations];
DROP TABLE IF EXISTS [dbo].[Users];

CREATE TABLE [dbo].[Users] (
	[ID]								INT				NOT NULL	IDENTITY,
	[Username]							VARCHAR (16)	NOT NULL,
	[Password]							CHAR (64)		NOT NULL,
	[SecurityQuestion]					VARCHAR (127)	NOT NULL,
	[SecurityAnswer]					CHAR (64)		NOT NULL,
	[IsTimeLimitInactive]				BIT				NOT NULL,
	[IsTimeLimitOrderly]				BIT				NOT NULL,
	[TimeLimitFromTime]					TIME (7)		NOT NULL,
	[TimeLimitToTime]					TIME (7)		NOT NULL,
	[TimeLimitOccasionalMinutes]		INT				NOT NULL,
	[IsProgramLimitOrderly]				BIT				NOT NULL,
	[ProgramLimitFromTime]				TIME (7)		NOT NULL,
	[ProgramLimitToTime]				TIME (7)		NOT NULL,
	[ProgramLimitOccasionalMinutes]		INT				NOT NULL,

	PRIMARY KEY							CLUSTERED		([ID] ASC)
);

CREATE TABLE [dbo].[ProgramLimitations] (
	[ID]								INT				NOT NULL	IDENTITY,
	[UserID]							INT				NOT NULL,
	[Name]								VARCHAR (63)	NOT NULL,
	[Path]								VARCHAR (255)	NOT NULL,
	[IsFullLimit]						BIT				NOT NULL,

	PRIMARY KEY							CLUSTERED		([ID] ASC),
	FOREIGN KEY							([UserID])		REFERENCES	[dbo].[Users]		([ID])
);

CREATE TABLE [dbo].[Keywords] (
	[ID]								INT				NOT NULL	IDENTITY,
	[Name]								VARCHAR (255)	NOT NULL,

	PRIMARY KEY							CLUSTERED		([ID] ASC)
);

CREATE TABLE [dbo].[WebLimitations] (
	[ID]								INT				NOT NULL	IDENTITY,
	[UserID]							INT				NOT NULL,
	[KeywordID]							INT				NOT NULL,

	PRIMARY KEY							CLUSTERED		([ID] ASC),
	FOREIGN KEY							([UserID])		REFERENCES	[dbo].[Users]		([ID]),
	FOREIGN KEY							([KeywordID])	REFERENCES	[dbo].[Keywords]	([ID])
);