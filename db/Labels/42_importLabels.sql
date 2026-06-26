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
INSERT INTO base.langcode (code, descr)
VALUES
('en', 'English'),
('de', 'Deutsch'),
('it', 'Italian'),
('es', 'Español'),
('ma', 'Maori');

-- Testing codes
INSERT INTO base.langcode (code, descr) VALUES
('c1', 'xxx'),
('c2', 'xxx'),
('c3', 'xxx'),
('c4', 'xxx'),
('c5', 'xxx'),
('c6', 'xxx'),
('c7', 'xxx'),
('c8', 'xxx');

UPDATE base.langcode SET isactive = FALSE WHERE code = 'c3';

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
    langcode TEXT NOT NULL,
    langkey  TEXT NOT NULL,
    label    TEXT,
    tooltip  TEXT
);

-- -------------------------------------------------
-- Import files (psql client-side COPY)
-- -------------------------------------------------

\copy zz_import_labels_base FROM 'C:/Source/db/Labels/BaseEn.txt' WITH (FORMAT csv);
INSERT INTO zz_import_labels_basex (langcode, langkey, label)
SELECT 'en', langkey, label FROM zz_import_labels_base;
TRUNCATE zz_import_labels_base;

\copy zz_import_labels_base FROM 'C:/Source/db/Labels/BaseDe.txt' WITH (FORMAT csv);
INSERT INTO zz_import_labels_basex (langcode, langkey, label)
SELECT 'de', langkey, label FROM zz_import_labels_base;
TRUNCATE zz_import_labels_base;

\copy zz_import_labels_base FROM 'C:/Source/db/Labels/BaseTtEn.txt' WITH (FORMAT csv);
INSERT INTO zz_import_labels_basex (langcode, langkey, tooltip)
SELECT 'en', langkey, label FROM zz_import_labels_base;
TRUNCATE zz_import_labels_base;

\copy zz_import_labels_base FROM 'C:/Source/db/Labels/BaseTtDe.txt' WITH (FORMAT csv);
INSERT INTO zz_import_labels_basex (langcode, langkey, tooltip)
SELECT 'de', langkey, label FROM zz_import_labels_base;
TRUNCATE zz_import_labels_base;

-- -------------------------------------------------
-- Cleanup
-- -------------------------------------------------
DELETE FROM zz_import_labels_basex WHERE langkey = '<LAST>';

UPDATE zz_import_labels_basex
SET label = REPLACE(label, E'||', ',')
WHERE label LIKE '%' || E'||' || '%';

UPDATE zz_import_labels_basex
SET tooltip = REPLACE(tooltip, E'||', ',')
WHERE tooltip LIKE '%' || E'||' || '%';

-- Fill missing tooltips
UPDATE zz_import_labels_basex b
SET tooltip = x.tooltip
FROM zz_import_labels_basex x
WHERE b.tooltip IS NULL
  AND x.tooltip IS NOT NULL
  AND b.langcode = x.langcode
  AND b.langkey = x.langkey;

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
    x.langcode,
    x.langkey,
    x.label     AS code,
    x.tooltip
FROM zz_import_labels_basex x
JOIN base.langkey lk ON lk.code = x.langkey;

INSERT INTO base.langlabel (langkeyid, langcode, code, tooltip)
SELECT DISTINCT langkeyid, langcode, code, tooltip
FROM zz_langlabel;

-- -------------------------------------------------
-- NULL cleanup
-- -------------------------------------------------
UPDATE base.langlabel SET tooltip = NULL WHERE tooltip = 'NULL';

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