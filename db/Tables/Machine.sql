DECLARE @NewID TABLE (NewId INT);
DECLARE @Descrim AS NVARCHAR(10);

DROP TABLE IF EXISTS Machine
SET @Descrim = 'mach';
DELETE FROM _BaseEntity WHERE Descrim = @Descrim;

CREATE TABLE Machine (
    Id         INT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	_Id        INT             NOT NULL,
	Stations   INT             NOT NULL,
	FOREIGN KEY (_Id) REFERENCES _BaseEntity(Id)
)


INSERT INTO _BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'M1', 'Machine 1') ;
INSERT INTO Machine (_Id, Stations) VALUES ((select NewId from @NewID), 15);
DELETE FROM @NewID;

INSERT INTO _BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'M2', 'Machine 2');
INSERT INTO Machine (_Id, Stations) VALUES ((select NewId from @NewID), 15);
DELETE FROM @NewID;

INSERT INTO _BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'M3', 'Machine 3');
INSERT INTO Machine (_Id, Stations) VALUES ((select NewId from @NewID), 30);
DELETE FROM @NewID;

select * from _baseEntity
select * from Machine
