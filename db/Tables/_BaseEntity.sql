DROP TABLE IF EXISTS Machine
DROP TABLE IF EXISTS _BaseEntity

CREATE TABLE _BaseEntity (
	_OrgId     INT             NOT NULL,
    Id         INT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	Descrim    NVARCHAR (100)  NOT NULL,
	Code       NVARCHAR (100)  NOT NULL,
	Descr      NVARCHAR (MAX)  NOT NULL,
	Encoded    NVARCHAR (MAX)  NULL,
	Updated    DATETIME        NOT NULL DEFAULT GETDATE(),

	FOREIGN KEY (_OrgId) REFERENCES _Org(Id)
)

select * from _BaseEntity