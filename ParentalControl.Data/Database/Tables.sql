DROP TABLE IF EXISTS [dbo].[WebSettings];
DROP TABLE IF EXISTS [dbo].[Keywords];
DROP TABLE IF EXISTS [dbo].[TimeSettings];
DROP TABLE IF EXISTS [dbo].[ProgramSettings];
DROP TABLE IF EXISTS [dbo].[Users];

CREATE TABLE [dbo].[Users] (
	[ID]					INT				NOT NULL,
	[Username]				VARCHAR (127)	NOT NULL,
	[Password]				VARCHAR (127)	NOT NULL,
	[SecurityQuestion]		VARCHAR (127)	NOT NULL,
	[SecurityAnswer]		VARCHAR (127)	NOT NULL,

	PRIMARY KEY				CLUSTERED		([ID] ASC)
);

CREATE TABLE [dbo].[ProgramSettings] (
	[ID]					INT				NOT NULL,
	[UserID]				INT				NOT NULL,
	[Name]					VARCHAR (63)	NOT NULL,
	[Path]					VARCHAR (255)	NOT NULL,
	[Occasional]			BIT				NOT NULL,
	[Minutes]				INT				NOT NULL,
	[Repeat]				BIT				NOT NULL,
	[Pause]					INT				NOT NULL,
	[Quantity]				INT				NOT NULL,
	[Orderly]				BIT				NOT NULL,
	[FromTime]				TIME (7)		NOT NULL,
	[ToTime]				TIME (7)		NOT NULL,

	PRIMARY KEY				CLUSTERED		([ID] ASC),
	FOREIGN KEY				([UserID])		REFERENCES	[dbo].[Users]		([ID])
);

CREATE TABLE [dbo].[TimeSettings] (
	[ID]					INT				NOT NULL,
	[UserID]				INT				NOT NULL,
	[Occasional]			BIT				NOT NULL,
	[Minutes]				INT				NOT NULL,
	[Orderly]				BIT				NOT NULL,
	[FromTime]				TIME (7)		NOT NULL,
	[ToTime]				TIME (7)		NOT NULL,

	PRIMARY KEY				CLUSTERED		([ID] ASC),
	FOREIGN KEY				([UserID])		REFERENCES	[dbo].[Users]		([ID])
);

CREATE TABLE [dbo].[Keywords] (
	[ID]					INT				NOT NULL,
	[Name]					VARCHAR (255)	NOT NULL,

	PRIMARY KEY				CLUSTERED		([ID] ASC)
);

CREATE TABLE [dbo].[WebSettings] (
	[ID]					INT				NOT NULL,
	[UserID]				INT				NOT NULL,
	[KeywordID]				INT				NOT NULL,

	PRIMARY KEY				CLUSTERED		([ID] ASC),
	FOREIGN KEY				([UserID])		REFERENCES	[dbo].[Users]		([ID]),
	FOREIGN KEY				([KeywordID])	REFERENCES	[dbo].[Keywords]	([ID])
);