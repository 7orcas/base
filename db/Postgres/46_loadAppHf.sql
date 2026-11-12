\set ON_ERROR_STOP on
BEGIN;

INSERT INTO hf.trailGroup (id, orgnr, code, descr)
VALUES
(1, 1, 'TG1', 'Trail Group 1'),
(2, 2, 'TG2', 'Trail Group 2');

INSERT INTO hf.trail
    (id, orgnr, trailGroupid, code, descr)
VALUES
(1,   1, 1, 'Trail1',  'Trail 1 desc'),
(2,   1, 1, 'Trail2',  'Trail 2 desc');



-- -------------------------------------------------
-- Fix identity sequences
-- -------------------------------------------------
SELECT setval(
    pg_get_serial_sequence('hf.trailGroup', 'id'),
    (SELECT MAX(id) FROM hf.trailGroup)
);

SELECT setval(
    pg_get_serial_sequence('hf.trail', 'id'),
    (SELECT MAX(id) FROM hf.trail)
);



COMMIT;