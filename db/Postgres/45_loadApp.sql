\set ON_ERROR_STOP on
BEGIN;

-- -------------------------------------------------
-- app.machinegroup
-- -------------------------------------------------
INSERT INTO app.machinegroup (id, orgnr, code, descr)
VALUES
(1, 1, 'MG1', 'Machine Group 1'),
(2, 2, 'MG2', 'Machine Group 2');

-- -------------------------------------------------
-- app.machine
-- -------------------------------------------------
INSERT INTO app.machine
    (id, orgnr, machinegroupid, stationpairs, code, descr, classification)
VALUES
(1,   1, 1, 12, 'M1-1',  'Machine 11', 2),
(2,   1, 1, 15, 'M1-2',  'Machine 12', NULL),
(100, 2, 2, 12, 'M2-1',  'Machine 21', 2),
(102, 2, 2, 15, 'M2-2',  'Machine 22', NULL),
(103, 1, 2, 12, 'M2-1',  'Machine 21', NULL),
(104, 1, 2, 15, 'M4-2',  'Machine 22', NULL),
(105, 1, 2, 12, 'M5-1',  'Machine 21', NULL),
(106, 1, 2, 15, 'M6-2',  'Machine 22', NULL),
(107, 1, 2, 12, 'M7-1',  'Machine 21', NULL),
(108, 1, 2, 15, 'M8-2',  'Machine 22', NULL),
(109, 1, 2, 12, 'M9-1',  'Machine 21', NULL),
(110, 1, 2, 15, 'M10-2', 'Machine 22', NULL);



-- -------------------------------------------------
-- Fix identity sequences
-- -------------------------------------------------
SELECT setval(
    pg_get_serial_sequence('app.machinegroup', 'id'),
    (SELECT MAX(id) FROM app.machinegroup)
);

SELECT setval(
    pg_get_serial_sequence('app.machine', 'id'),
    (SELECT MAX(id) FROM app.machine)
);


/*

INSERT INTO country (id,reftype_id,sort,code,descr,image,org_nr,updated,updated_userid) VALUES (10010,10001,1,'nz','New Zealand',   'nz.png',1,current_timestamp,1);
INSERT INTO country (id,reftype_id,sort,code,descr,image,org_nr,updated,updated_userid) VALUES (10011,10001,2,'au','Australia',     'au.png',1,current_timestamp,1);
INSERT INTO country (id,reftype_id,sort,code,descr,image,org_nr,updated,updated_userid,active) VALUES (10012,10001,3,'uk','United Kingdom','uk.png',1,current_timestamp,1,false);

INSERT INTO currency (id,reftype_id,sort,code,descr,org_nr,updated,updated_userid) VALUES (10020,10002,3,'NZD','New Zealand Dollar' ,1,current_timestamp,1);
INSERT INTO currency (id,reftype_id,sort,code,descr,org_nr,updated,updated_userid) VALUES (10021,10002,2,'AUD','Australian Dollar',  1,current_timestamp,1);
INSERT INTO currency (id,reftype_id,sort,code,descr,org_nr,updated,updated_userid) VALUES (10022,10002,1,'GBP','Great Britain Pound',1,current_timestamp,1);
*/


COMMIT;