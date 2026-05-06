BEGIN TRANSACTION;

SET IDENTITY_INSERT app.machineGroup ON;
INSERT INTO app.machineGroup (id, orgNr, code, descr) VALUES (1, 1, 'MG1', 'Machine Group 1');
INSERT INTO app.machineGroup (id, orgNr, code, descr) VALUES (2, 2, 'MG2', 'Machine Group 2');
SET IDENTITY_INSERT app.machineGroup OFF;

SET IDENTITY_INSERT app.machine ON;
INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr, classification) VALUES (1, 1, 1, 12, 'M1-1', 'Machine 11',2);
INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (2, 1, 1, 15, 'M1-2', 'Machine 12');

INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr, classification) VALUES (100, 2, 2, 12, 'M2-1', 'Machine 21',2);
INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (102, 2, 2, 15, 'M2-2', 'Machine 22');

INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (103, 1, 2, 12, 'M2-1', 'Machine 21');
INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (104, 1, 2, 15, 'M4-2', 'Machine 22');

INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (105, 1, 2, 12, 'M5-1', 'Machine 21');
INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (106, 1, 2, 15, 'M6-2', 'Machine 22');

INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (107, 1, 2, 12, 'M7-1', 'Machine 21');
INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (108, 1, 2, 15, 'M8-2', 'Machine 22');

INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (109, 1, 2, 12, 'M9-1', 'Machine 21');
INSERT INTO app.machine (id, orgNr, machineGroupId, stationPairs, code, descr) VALUES (110, 1, 2, 15, 'M10-2', 'Machine 22');

SET IDENTITY_INSERT app.machine OFF;



/*

INSERT INTO country (id,reftype_id,sort,code,descr,image,org_nr,updated,updated_userid) VALUES (10010,10001,1,'nz','New Zealand',   'nz.png',1,current_timestamp,1);
INSERT INTO country (id,reftype_id,sort,code,descr,image,org_nr,updated,updated_userid) VALUES (10011,10001,2,'au','Australia',     'au.png',1,current_timestamp,1);
INSERT INTO country (id,reftype_id,sort,code,descr,image,org_nr,updated,updated_userid,active) VALUES (10012,10001,3,'uk','United Kingdom','uk.png',1,current_timestamp,1,false);

INSERT INTO currency (id,reftype_id,sort,code,descr,org_nr,updated,updated_userid) VALUES (10020,10002,3,'NZD','New Zealand Dollar' ,1,current_timestamp,1);
INSERT INTO currency (id,reftype_id,sort,code,descr,org_nr,updated,updated_userid) VALUES (10021,10002,2,'AUD','Australian Dollar',  1,current_timestamp,1);
INSERT INTO currency (id,reftype_id,sort,code,descr,org_nr,updated,updated_userid) VALUES (10022,10002,1,'GBP','Great Britain Pound',1,current_timestamp,1);
*/


COMMIT;