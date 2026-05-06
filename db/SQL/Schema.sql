DROP TABLE IF EXISTS Machine
DROP TABLE IF EXISTS MouldGroup
DROP TABLE IF EXISTS App.BaseEntity
DROP TABLE IF EXISTS App.Org
DROP TABLE IF EXISTS App.Label
GO

DROP SCHEMA IF EXISTS App
GO

CREATE SCHEMA App 
GO

CREATE TABLE App.Org (
    Id         INT             PRIMARY KEY NOT NULL,
	Code       NVARCHAR (100)  NOT NULL,
	Descr      NVARCHAR (MAX)  NOT NULL
)
CREATE TABLE App.BaseEntity (
	_OrgId     INT             NOT NULL,
    Id         INT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	Descrim    NVARCHAR (100)  NOT NULL,
	Code       NVARCHAR (100)  NOT NULL,
	Descr      NVARCHAR (MAX)  NULL,
	Encoded    NVARCHAR (MAX)  NULL,
	Updated    DATETIME        NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY (_OrgId) REFERENCES App.Org(Id)
)
CREATE TABLE App.Label (
    Id         INT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	Lang       NVARCHAR (4)    NOT NULL,
	Code       NVARCHAR (100)  NOT NULL,
	_OrgId     INT             NULL,
	Descr      NVARCHAR (MAX)  NOT NULL,
	Tooltip    NVARCHAR (MAX)  NULL,
	Encoded    NVARCHAR (MAX)  NULL,
	Updated    DATETIME        NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY (_OrgId) REFERENCES App.Org(Id),
	CONSTRAINT Lable_Code UNIQUE (Lang, Code, _OrgId)
)


CREATE TABLE Machine (
    Id           INT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	_Id          INT             NOT NULL,
	StationPairs INT             NOT NULL,
	FOREIGN KEY (_Id) REFERENCES App.BaseEntity(Id)
)
CREATE TABLE MouldGroup (
    Id         INT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	_Id        INT             NOT NULL,
	FOREIGN KEY (_Id) REFERENCES App.BaseEntity(Id)
);
