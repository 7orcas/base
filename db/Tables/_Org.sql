DROP TABLE IF EXISTS _Org

CREATE TABLE _Org (
    Id         INT             PRIMARY KEY NOT NULL,
	Code       NVARCHAR (100)  NOT NULL,
	Descr      NVARCHAR (MAX)  NOT NULL
)

INSERT INTO _Org (Id, Code, Descr) VALUES (1, 'Org1', 'Organisation 1');
INSERT INTO _Org (Id, Code, Descr) VALUES (2, 'Org2', 'Organisation 2');

select * from _Org