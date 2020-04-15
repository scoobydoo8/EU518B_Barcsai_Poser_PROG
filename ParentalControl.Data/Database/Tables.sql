DROP TABLE IF EXISTS [dbo].[WebLimitations];
DROP TABLE IF EXISTS [dbo].[Keywords];
DROP TABLE IF EXISTS [dbo].[ProgramLimitations];
DROP TABLE IF EXISTS [dbo].[Users];

CREATE TABLE [dbo].[Users] (
	[ID]						INT				NOT NULL	IDENTITY,
	[Username]					VARCHAR (16)	NOT NULL,
	[Password]					CHAR (64)		NOT NULL,
	[SecurityQuestion]			VARCHAR (127)	NOT NULL,
	[SecurityAnswer]			CHAR (64)		NOT NULL,
	[IsTimeLimitationActive]	BIT				NOT NULL,
	[Occasional]				BIT				NOT NULL,
	[Minutes]					INT				NOT NULL,
	[Orderly]					BIT				NOT NULL,
	[FromTime]					TIME (7)		NOT NULL,
	[ToTime]					TIME (7)		NOT NULL,

	PRIMARY KEY					CLUSTERED		([ID] ASC)
);

CREATE TABLE [dbo].[ProgramLimitations] (
	[ID]						INT				NOT NULL	IDENTITY,
	[UserID]					INT				NOT NULL,
	[Name]						VARCHAR (63)	NOT NULL,
	[Path]						VARCHAR (255)	NOT NULL,
	[Occasional]				BIT				NOT NULL,
	[Minutes]					INT				NOT NULL,
	[Repeat]					BIT				NOT NULL,
	[Pause]						INT				NOT NULL,
	[Quantity]					INT				NOT NULL,
	[Orderly]					BIT				NOT NULL,
	[FromTime]					TIME (7)		NOT NULL,
	[ToTime]					TIME (7)		NOT NULL,

	PRIMARY KEY					CLUSTERED		([ID] ASC),
	FOREIGN KEY					([UserID])		REFERENCES	[dbo].[Users]		([ID])
);

CREATE TABLE [dbo].[Keywords] (
	[ID]						INT				NOT NULL	IDENTITY,
	[Name]						VARCHAR (255)	NOT NULL,

	PRIMARY KEY					CLUSTERED		([ID] ASC)
);

CREATE TABLE [dbo].[WebLimitations] (
	[ID]						INT				NOT NULL	IDENTITY,
	[UserID]					INT				NOT NULL,
	[KeywordID]					INT				NOT NULL,

	PRIMARY KEY					CLUSTERED		([ID] ASC),
	FOREIGN KEY					([UserID])		REFERENCES	[dbo].[Users]		([ID]),
	FOREIGN KEY					([KeywordID])	REFERENCES	[dbo].[Keywords]	([ID])
);