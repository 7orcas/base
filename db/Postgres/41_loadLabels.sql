\set ON_ERROR_STOP on
BEGIN;

-- -------------------------------------------------
-- base.langcode
-- -------------------------------------------------
INSERT INTO base.langcode (id, code, descr, flag)
VALUES
(1, 'en', 'English', 'gb.svg'),
(2, 'de', 'Deutsch', 'de.svg'),
(3, 'it', 'Italian', 'it.svg'),
(4, 'es', 'Español', 'es.svg'),
(5, 'ma', 'Maori', 'Maori.png');


-- -------------------------------------------------
-- base.langkey
-- -------------------------------------------------
INSERT INTO base.langkey (id, code)
VALUES
(1, 'test1'),
(2, 'test2'),
(3, 'apiError');

-- -------------------------------------------------
-- base.langlabel
-- -------------------------------------------------
INSERT INTO base.langlabel (id, langkeyid, langcode, code, tooltip)
VALUES
(1, 1, 'en', 'TEST1 EN', 'TEST 1 TT EN'),
(2, 1, 'de', 'TEST1 DE', 'TEST 1 TT DE'),
(3, 2, 'en', 'TEST2 EN', 'TEST 2 TT EN'),
(4, 2, 'de', 'TEST2 DE', 'TEST 2 TT DE'),
(5, 3, 'en', 'Oops, something went wrong', NULL);

-- -------------------------------------------------
-- Fix identity sequences
-- -------------------------------------------------
SELECT setval(pg_get_serial_sequence('base.langcode', 'id'),
              (SELECT MAX(id) FROM base.langcode));

SELECT setval(pg_get_serial_sequence('base.langkey', 'id'),
              (SELECT MAX(id) FROM base.langkey));

SELECT setval(pg_get_serial_sequence('base.langlabel', 'id'),
              (SELECT MAX(id) FROM base.langlabel));

COMMIT;