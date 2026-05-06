DECLARE @NewID TABLE (NewId INT);

DELETE FROM MouldGroup
DELETE FROM Machine
DELETE FROM _base.Label
DELETE FROM _base.Org

INSERT INTO _base.Org (Id, Code, Descr) VALUES (1, 'Org1', 'Organisation 1');
INSERT INTO _base.Org (Id, Code, Descr) VALUES (2, 'Org2', 'Organisation 2');

INSERT INTO _base.machine (id, stationPairs, _OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'M1', 'Machine 1') ;
INSERT INTO Machine (_Id, StationPairs) VALUES ((select NewId from @NewID), 15);
DELETE FROM @NewID;
INSERT INTO _base.BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'M2', 'Machine 2');
INSERT INTO Machine (_Id, StationPairs) VALUES ((select NewId from @NewID), 15);
DELETE FROM @NewID;
INSERT INTO _base.BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'M3', null);
INSERT INTO Machine (_Id, StationPairs) VALUES ((select NewId from @NewID), 30);
DELETE FROM @NewID;

SET @Descrim = 'mldg';
INSERT INTO _base.BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'MG1', 'Mould Group 1') ;
INSERT INTO MouldGroup (_Id) VALUES ((select NewId from @NewID));
DELETE FROM @NewID;
INSERT INTO _base.BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'MG2', 'Mould Group 2');
INSERT INTO MouldGroup (_Id) VALUES ((select NewId from @NewID));
DELETE FROM @NewID;
INSERT INTO _base.BaseEntity (_OrgId, Descrim, Code,	Descr) OUTPUT INSERTED.Id INTO @NewId VALUES (1, @Descrim, 'MG3', null);
INSERT INTO MouldGroup (_Id) VALUES ((select NewId from @NewID));
DELETE FROM @NewID;

INSERT INTO _base.Label (Lang, Code,	Descr, Tooltip) SELECT Lang, Code,	Descr, Tooltip FROM zLabel

/*
INSERT INTO _base.Label (Lang, Code,	Descr, Tooltip) VALUES ('en', '123', '*EN 123 TEST*', '123 tooltip') ;
INSERT INTO _base.Label (Lang, Code,	Descr, _OrgId, Tooltip) VALUES ('en', '123', '*EN 123 Org=1 TEST*', 1, '123 org 1 tooltip') ;
INSERT INTO _base.Label (Lang, Code,	Descr) VALUES ('de', '123', '*DE 123 TEST*') ;

INSERT INTO _base.Label (Lang, Code,	Descr, Tooltip) VALUES ('en', 'yes', 'Yes', 'Yes tooltip') ;
INSERT INTO _base.Label (Lang, Code,	Descr, _OrgId) VALUES ('en', 'yes', 'Ok', 1) ;
INSERT INTO _base.Label (Lang, Code,	Descr) VALUES ('de', 'yes', 'Ja') ;

INSERT INTO _base.Label (Lang, Code,	Descr) VALUES ('en', 'no', 'No') ;

INSERT INTO _base.Label (Lang, Code,	Descr, Tooltip) VALUES ('en', 'langCode', 'Lang', 'Language Code') ;
INSERT INTO _base.Label (Lang, Code,	Descr) VALUES ('de', 'langCode', 'Sprache') ;

INSERT INTO _base.Label (Lang, Code,	Descr, Tooltip) VALUES ('en', 'Org', 'Org', 'Organisation Id') ;
INSERT INTO _base.Label (Lang, Code,	Descr) VALUES ('en', 'Code', 'Code') ;
INSERT INTO _base.Label (Lang, Code,	Descr) VALUES ('en', 'Label', 'Label') ;
INSERT INTO _base.Label (Lang, Code,	Descr) VALUES ('en', 'Tooltip', 'Tooltip') ;
*/

--select * from _base.Label
select * from _base.BaseEntity
--select * from MouldGroup
--select * from Machine