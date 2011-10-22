CREATE DATABASE [dbss];
GO

USE [dbss]

CREATE TABLE Sheet (
	[x] INT,
	[y] INT,
	[Name] NVARCHAR(MAX),
	[Formula] NVARCHAR(MAX),
	[Hash] BIGINT PRIMARY KEY
);

CREATE TABLE Depends (
	[Hash] BIGINT,
	[Depends] BIGINT
);
GO
CREATE NONCLUSTERED INDEX depends_hash ON Depends([Hash])
CREATE NONCLUSTERED INDEX depends_depends ON Depends([Depends])

CREATE TABLE Caches (
	[Hash] BIGINT PRIMARY KEY,
	[RPN] NVARCHAR(MAX),
	[Value] NVARCHAR(MAX)
);