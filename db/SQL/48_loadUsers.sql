INSERT INTO _cntrl.zzz (id, xxx, yyy, org_nr, orgs, attempts, updated, active) VALUES (10000, 'js@7orcas.com', '123', 0, '1,2,3', 0, current_timestamp, true);
INSERT INTO _cntrl.zzz (id, xxx, yyy, org_nr, orgs, attempts, updated, active) VALUES (10001, '111',           '111', 0, '1,2,3', 1, current_timestamp, true);
INSERT INTO _cntrl.zzz (id, xxx, yyy, org_nr, orgs, attempts, updated, active) VALUES (10002, '222',           '222', 0, '1',     2, current_timestamp, true);
INSERT INTO _cntrl.zzz (id, xxx, yyy, org_nr, orgs, attempts, updated, active) VALUES (10003, '333',           '333', 0, '1',     3, current_timestamp, false);
INSERT INTO _cntrl.zzz (id, xxx, yyy, org_nr, orgs, attempts, updated, active) VALUES (10004, '444',           '444', 0, '1',     4, current_timestamp, true);

INSERT INTO _cntrl.zzz_role (id, zzz_id, role_id, updated) VALUES (10000, 10000, 1,     current_timestamp);
INSERT INTO _cntrl.zzz_role (id, zzz_id, role_id, updated) VALUES (10001, 10001, 2,     current_timestamp);
INSERT INTO _cntrl.zzz_role (id, zzz_id, role_id, updated) VALUES (10002, 10001, 3,     current_timestamp);
INSERT INTO _cntrl.zzz_role (id, zzz_id, role_id, updated) VALUES (10003, 10001, 10000, current_timestamp);
INSERT INTO _cntrl.zzz_role (id, zzz_id, role_id, updated) VALUES (10004, 10002, 2,     current_timestamp);
INSERT INTO _cntrl.zzz_role (id, zzz_id, role_id, updated) VALUES (10005, 10002, 3,     current_timestamp);
