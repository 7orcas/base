DELETE FROM base.langCode;
DELETE FROM base.langLabel;
DELETE FROM base.langKey;

INSERT INTO base.langCode (code,descr) VALUES ('en','English');
INSERT INTO base.langCode (code,descr) VALUES ('de','Deutsch');
INSERT INTO base.langCode (code,descr) VALUES ('it','Italian');
INSERT INTO base.langCode (code,descr) VALUES ('es',N'Español');
INSERT INTO base.langCode (code,descr) VALUES ('ma','Maori');
--TESTING
insert into base.langCode (code,descr) select 'c1', 'xxx'
insert into base.langCode (code,descr) select 'c2', 'xxx'
insert into base.langCode (code,descr,isActive) select 'c3', 'xxx', 0
insert into base.langCode (code,descr) select 'c4', 'xxx'
insert into base.langCode (code,descr) select 'c5', 'xxx'
insert into base.langCode (code,descr) select 'c6', 'xxx'
insert into base.langCode (code,descr) select 'c7', 'xxx'
insert into base.langCode (code,descr) select 'c8', 'xxx'

DROP TABLE IF EXISTS zzImportLabelsBase;
DROP TABLE IF EXISTS zzImportLabelsBaseX;
DROP TABLE IF EXISTS zzLangLabel;

--Base Labels
CREATE TABLE zzImportLabelsBase (
	langKey   NVARCHAR (100)  NOT NULL,
	label       NVARCHAR (MAX)  NOT NULL
);
CREATE TABLE zzImportLabelsBaseX (
	langCode   NVARCHAR (100)  NOT NULL,
    langKey   NVARCHAR (100)  NOT NULL,
	label       NVARCHAR (MAX)  NULL,
	tooltip       NVARCHAR (MAX)  NULL
);

BULK INSERT zzImportLabelsBase FROM 'C:\src\f2f\db\Labels\BaseEn.txt' WITH (FIELDTERMINATOR = ',', ROWTERMINATOR = '\n', FIRSTROW = 1);
INSERT INTO zzImportLabelsBaseX (langCode,langKey,label) SELECT 'en',langKey,label FROM zzImportLabelsBase;
DELETE FROM zzImportLabelsBase;

BULK INSERT zzImportLabelsBase FROM 'C:\src\f2f\db\Labels\BaseDe.txt' WITH (FIELDTERMINATOR = ',', ROWTERMINATOR = '\n', FIRSTROW = 1);
INSERT INTO zzImportLabelsBaseX (langCode,langKey,label) SELECT 'de',langKey,label FROM zzImportLabelsBase;
DELETE FROM zzImportLabelsBase;

BULK INSERT zzImportLabelsBase FROM 'C:\src\f2f\db\Labels\BaseTtEn.txt' WITH (FIELDTERMINATOR = ',', ROWTERMINATOR = '\n', FIRSTROW = 1);
INSERT INTO zzImportLabelsBaseX (langCode,langKey,tooltip) SELECT 'en',langKey,label FROM zzImportLabelsBase;
DELETE FROM zzImportLabelsBase;

BULK INSERT zzImportLabelsBase FROM 'C:\src\f2f\db\Labels\BaseTtDe.txt' WITH (FIELDTERMINATOR = ',', ROWTERMINATOR = '\n', FIRSTROW = 1);
INSERT INTO zzImportLabelsBaseX (langCode,langKey,tooltip) SELECT 'de',langKey,label FROM zzImportLabelsBase;
DELETE FROM zzImportLabelsBase;

DELETE FROM zzImportLabelsBaseX WHERE langKey = '<LAST>';

UPDATE zzImportLabelsBaseX SET label = REPLACE (label, '||', ',') WHERE label LIKE '%||%';
UPDATE zzImportLabelsBaseX SET tooltip = REPLACE (tooltip, '||', ',') WHERE tooltip LIKE '%||%';

UPDATE zzImportLabelsBaseX 
	SET tooltip = (SELECT TOP 1 tooltip 
					FROM zzImportLabelsBaseX x 
					WHERE x.langCode = zzImportLabelsBaseX.langCode 
					AND x.langKey = zzImportLabelsBaseX.langKey 
					AND x.tooltip IS NOT NULL
					) WHERE zzImportLabelsBaseX.tooltip IS NULL;

DELETE FROM zzImportLabelsBaseX WHERE label IS NULL;

--select * from zzImportLabelsBaseX where langKey = 'SaveRec'

INSERT INTO base.langKey (code) SELECT DISTINCT langKey FROM zzImportLabelsBaseX;

SELECT 0 AS langKeyId
		,langCode
		,langKey
		,label AS code
		,tooltip
	INTO zzLangLabel
	FROM zzImportLabelsBaseX;
	
UPDATE zzLangLabel SET langKeyId = 
	(SELECT l.id FROM base.langKey l
	 WHERE l.code = zzLangLabel.langKey)

--select * from zzImportLabelsBase


INSERT INTO base.langLabel (langKeyId,langCode,code,tooltip)
	SELECT DISTINCT langKeyId,langCode,code,tooltip FROM zzLangLabel;

DELETE FROM zImportLabels WHERE code IN (SELECT code FROM base.langKey)

--mff Labels
DROP TABLE IF EXISTS zzLangLabel;

INSERT INTO base.langKey (code)
	SELECT DISTINCT Code FROM zImportLabels;


SELECT 0 AS langKeyId
		,Lang AS langCode
		,Code AS langKey
		,Descr AS code
		,Tooltip AS tooltip
	INTO zzLangLabel
	FROM zImportLabels;
	
UPDATE zzLangLabel SET langKeyId = 
	(SELECT l.id FROM base.langKey l
	 WHERE l.code = zzLangLabel.langKey)

INSERT INTO base.langLabel (langKeyId,langCode,code,tooltip)
	SELECT DISTINCT langKeyId,langCode,code,tooltip FROM zzLangLabel;

UPDATE base.langLabel SET Tooltip = null WHERE Tooltip = 'NULL';

DROP TABLE IF EXISTS zzImportLabelsBaseEn;
DROP TABLE IF EXISTS zzImportLabelsBaseDe;
DROP TABLE IF EXISTS zzLangLabel;

SELECT 'Keys', Count(*) FROM base.langKey;

/*
SELECT k.code, l.*
	FROM base.langKey k
	LEFT JOIN base.langLabel l on l.langKeyId = k.id
	ORDER BY k.code 
*/