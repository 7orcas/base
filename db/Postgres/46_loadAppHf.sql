\set ON_ERROR_STOP on
BEGIN;

INSERT INTO hf.trialGroup (id, orgnr, code, descr) VALUES
 (1, 1, 'TG1', 'Trail Group 1'),
 (2, 2, 'TG2', 'Trail Group 2');

INSERT INTO hf.trialGroupUserAcc (id, trialGroupId, userAccId, isadmin) VALUES
 (1, 1,  1,   true),
 (2, 1,  111, false),
 (20, 2, 111, true);

INSERT INTO hf.trial (id, orgnr, trialGroupid, code, descr, status) VALUES
(1,   1, 1, 'Trail1',  'Trail 1 desc', 0),
(2,   1, 1, 'Trail2',  'Trail 2 desc', 0);


-- -------------------------------------------------
-- Fix identity sequences
-- -------------------------------------------------
SELECT setval(pg_get_serial_sequence('hf.trialGroup', 'id'),       (SELECT MAX(id) FROM hf.trialGroup));
SELECT setval(pg_get_serial_sequence('hf.trialGroupUserAcc', 'id'),(SELECT MAX(id) FROM hf.trialGroupUserAcc));
SELECT setval(pg_get_serial_sequence('hf.trial', 'id'),            (SELECT MAX(id) FROM hf.trial));


COMMIT;