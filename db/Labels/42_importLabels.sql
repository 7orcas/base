\set ON_ERROR_STOP on
BEGIN;

--NOT WORKING
-- \if :{?label_path}
--\else
--	\echo 'ERROR: label_path not provided'
--	\quit 1
--\endif
--\set label_path C:/Source/db/Labels
--\echo label_path = :'label_path';

-- -------------------------------------------------
-- Clear existing data
-- -------------------------------------------------
DELETE FROM base.langlabel;
DELETE FROM base.langkey;
DELETE FROM base.langcode;

-- -------------------------------------------------
-- Languages
-- -------------------------------------------------
INSERT INTO base.langcode (id, code, descr, flag)
VALUES
(1, 'en', 'English', 'gb.svg'),
(2, 'de', 'Deutsch', 'de.svg'),
(3, 'it', 'Italian', 'it.svg'),
(4, 'es', 'Español', 'es.svg'),
(5, 'ma', 'Maori', 'Maori.png'),
(6, 'cn', 'Chinese', 'cn.svg');

INSERT INTO base.langcode (id, code, descr, flag, variant)
VALUES
(10, 'en', 'English', 'gb.svg', 1);

-- Testing codes
--INSERT INTO base.langcode (code, descr) VALUES
--('c1', 'xxx'),
--('c2', 'xxx'),
--('c3', 'xxx'),
--('c4', 'xxx'),
--('c5', 'xxx'),
--('c6', 'xxx'),
--('c7', 'xxx'),
--('c8', 'xxx');
--UPDATE base.langcode SET isactive = FALSE WHERE code = 'c3';

-- -------------------------------------------------
-- Staging tables
-- -------------------------------------------------
DROP TABLE IF EXISTS zz_import_labels_base;
DROP TABLE IF EXISTS zz_import_labels_basex;
DROP TABLE IF EXISTS zz_langlabel;

CREATE TABLE zz_import_labels_base (
    langkey TEXT NOT NULL,
    label   TEXT NOT NULL
);

CREATE TABLE zz_import_labels_basex (
	langCodeId BIGINT NOT NULL,
    langcode TEXT NOT NULL,
    langkey  TEXT NOT NULL,
    label    TEXT
);

-- -------------------------------------------------
-- Import files (psql client-side COPY)
-- -------------------------------------------------

\copy zz_import_labels_base FROM 'C:/Source/db/Labels/BaseEn.txt' WITH (FORMAT csv);
INSERT INTO zz_import_labels_basex (langCodeId, langcode, langkey, label)
SELECT 1, 'en', langkey, label FROM zz_import_labels_base;
TRUNCATE zz_import_labels_base;

\copy zz_import_labels_base FROM 'C:/Source/db/Labels/BaseDe.txt' WITH (FORMAT csv);
INSERT INTO zz_import_labels_basex (langCodeId, langcode, langkey, label)
SELECT 2, 'de', langkey, label FROM zz_import_labels_base;
TRUNCATE zz_import_labels_base;

\copy zz_import_labels_base FROM 'C:/Source/db/Labels/BaseCn.txt' WITH (FORMAT csv);
INSERT INTO zz_import_labels_basex (langCodeId, langcode, langkey, label)
SELECT 6, 'cn', langkey, label FROM zz_import_labels_base;
TRUNCATE zz_import_labels_base;

\copy zz_import_labels_base FROM 'C:/Source/db/Labels/BaseMa.txt' WITH (FORMAT csv);
INSERT INTO zz_import_labels_basex (langCodeId, langcode, langkey, label)
SELECT 5, 'ma', langkey, label FROM zz_import_labels_base;
TRUNCATE zz_import_labels_base;

-- -------------------------------------------------
-- Cleanup
-- -------------------------------------------------
DELETE FROM zz_import_labels_basex WHERE langkey = '<LAST>';

UPDATE zz_import_labels_basex
SET label = REPLACE(label, E'||', ',')
WHERE label LIKE '%' || E'||' || '%';

DELETE FROM zz_import_labels_basex WHERE label IS NULL;

-- -------------------------------------------------
-- Base labels
-- -------------------------------------------------
INSERT INTO base.langkey (code)
SELECT DISTINCT langkey
FROM zz_import_labels_basex
ON CONFLICT (code) DO NOTHING;

CREATE TABLE zz_langlabel AS
SELECT
    lk.id       AS langkeyid,
    x.langcodeId,
    x.langkey,
    x.label     AS code
FROM zz_import_labels_basex x
JOIN base.langkey lk ON lk.code = x.langkey;

INSERT INTO base.langlabel (langcodeid, langkeyid, code)
SELECT DISTINCT langcodeid, langkeyid, code
FROM zz_langlabel;

-- -------------------------------------------------
-- NULL cleanup
-- -------------------------------------------------
--UPDATE base.langlabel SET tooltip = NULL WHERE tooltip = 'NULL';

-- -------------------------------------------------
-- Fix sequences
-- -------------------------------------------------
SELECT setval(pg_get_serial_sequence('base.langcode', 'id'),
              (SELECT MAX(id) FROM base.langcode));

SELECT setval(pg_get_serial_sequence('base.langkey', 'id'),
              (SELECT MAX(id) FROM base.langkey));

SELECT setval(pg_get_serial_sequence('base.langlabel', 'id'),
              (SELECT MAX(id) FROM base.langlabel));

COMMIT;