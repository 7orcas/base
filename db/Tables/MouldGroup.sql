DECLARE @NewId TABLE (NewId INT);
DECLARE @Descrim AS NVARCHAR(10);


DROP TABLE IF EXISTS MouldGroup;
SET @Descrim = 'mldg';
DELETE FROM _BaseEntity WHERE Descrim = @Descrim;

CREATE TABLE MouldGroup (
    Id         INT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	_Id        INT             NOT NULL,
	FOREIGN KEY (_Id) REFERENCES _BaseEntity(Id)
);


INSERT INTO _BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'MG1', 'Mould Group 1') ;
INSERT INTO MouldGroup (_Id) VALUES ((select NewId from @NewID));
DELETE FROM @NewID;

INSERT INTO _BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'MG2', 'Mould Group 2');
INSERT INTO MouldGroup (_Id) VALUES ((select NewId from @NewID));
DELETE FROM @NewID;

INSERT INTO _BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'MG3', 'Mould Group 3');
INSERT INTO MouldGroup (_Id) VALUES ((select NewId from @NewID));
DELETE FROM @NewID;


select * from _baseEntity
select * from MouldGroup