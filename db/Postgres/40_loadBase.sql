\set ON_ERROR_STOP on
BEGIN;

-- -------------------------------------------------
-- cntrl.loginoption
-- -------------------------------------------------
INSERT INTO cntrl.loginoption (urlsuffix, orgnr, orgnrs, langcode, langcodevariant, langcodes, isOrgDefault, ismasqueradeenabled, successaction) VALUES 
 ('base', 0, '0', 'en', 0, 'en', true, false, 0),
 ('blue', 1, '1', 'en', 0, 'en', true, false, 0),
 ('admin', 1, '1,2', 'en', 1, 'en,de,it,es,ma,cn', false, false, 0),
 ('service', 1, '1', 'en', 0, 'en', false, true, 0),
 ('all', 2, '1,2,3', 'de', 1, 'en,de,es,xx', true, true, 0),
 ('api', 1, '1', 'en', 0, 'en', false, false, 1),
 ('3', 3, '3', 'en', 0, 'en', false, false, 1);


-- -------------------------------------------------
-- base.org
-- -------------------------------------------------
INSERT INTO base.org (nr, code, typeOf, descr, langcode, langcodevariant) VALUES
 (0, 'Org Base', 'OrgEnt', 'Base Organisation', 'en', 0);

INSERT INTO base.org (nr, code, typeOf, descr, langcode, langcodevariant, mfa, 
	apikey,
    ispasswordresetenabled, issignupenabled, isemailrequired, isemailverified, ismasqueradeenabled,
	icon,
	encoded) VALUES
 (1, 'Blue', 'OrgEntHF', 'FYH', 'en', 1, 0, 
  'X123',	
  true, true, true, true, true, 
  '/images/Icons/BlueTransparentBlack512.png',
  '{Languages:[{LangCode:"en",IsEditable:true},{LangCode:"de",IsEditable:true},{LangCode:"ma",IsEditable:false},{LangCode:"es",IsEditable:false}]}');

INSERT INTO base.org (nr, code, typeOf, descr, langcode, langcodevariant, mfa, icon, encoded) VALUES
 (2, 'Org 2', 'OrgEntHF', 'Org 2 Description', 'en', 2, 0,
  '/images/Icons/BlueBlack512.png',
  '{Languages:[{LangCode:"de",IsEditable:true}]}');

INSERT INTO base.org (nr, code, typeOf, descr, langcode, langcodevariant, encoded) VALUES 
 (3, 'Org 3', 'OrgEntDS', 'Org 3 Description', 'de', 1,
  '{Languages:[{LangCode:"de",IsEditable:true}]}');


-- -------------------------------------------------
-- base.zzz
-- -------------------------------------------------
INSERT INTO base.zzz (id, xxx, yyy, email, orgnrdefault, langcode, isemailverified, ismfarequired, attempts) VALUES
 (1,  '1',      '1.bElHDRD8r/YSi60KfZ895Q==.OtZFpeplYRtPbhp1iDJv81XGo1hcP9okCCcDMK1l/ZQ=', 'john.stewart@7orcas.com', 1, 'en', true, false, 0),
 (16, 'user16', '1.V7GXs9KH9TTJSU7Bn0INQg==.Jmfr+bRc1P0FOXF7nw2XmnUJTmhA1TQsa5pu+6tidDc=', 'xx123@3.4',               1, 'en', true, false, 10),
 (2,  'user',   '1.V7GXs9KH9TTJSU7Bn0INQg==.Jmfr+bRc1P0FOXF7nw2XmnUJTmhA1TQsa5pu+6tidDc=', 'xx123@3.4',               1, 'en', true, false, 0),
 (-1, '$',      '1.yPDe3BhiQxj2zxJzX+Vtsw==.4vRBCyOlVw9v6OvWCm8c0nDOxv+sNy8Ly/bWujduO1o=', 'js@7orcas.com',           1, 'en', true, false, 0),
 (11, 'user11', '1.yOKi6N28RtoVof3RJ3ntuA==.T3melCMcS1TGsCKCqaRjvdPw8jxLMWPPE/ARJ9Kx2RM=', 'xx123@3.4',               1, 'en', true, false, 0),
 (12, 'Hōne',   '1.V7GXs9KH9TTJSU7Bn0INQg==.Jmfr+bRc1P0FOXF7nw2XmnUJTmhA1TQsa5pu+6tidDc=', 'xx123@3.4',               1, 'en', true, false, 0);
 
-- -------------------------------------------------
-- base.useracc
-- -------------------------------------------------
INSERT INTO base.useracc (id, zzzid, orgnr) VALUES
 (1,  -1,   1),
 (2,   1,   1),
 (11,  1,   2),
 (200, 2,   1),
 (300, 2,   2),
 (111, 11,  1),
 (112, 12,  1);

-- -------------------------------------------------
-- base.role
-- -------------------------------------------------
INSERT INTO base.role (id, orgnr, code, descr) VALUES
 (1,   1, 'Admin',   'Full Admin Access 1'),
 (101, 2, 'Admin',   'Full Admin Access 2'),
 (2,   1, 'Org RO',   null),
 (3,   1, 'LangEdit', null),
 (5,   1, 'Machines1',null),
 (10,  1, 'Trails',   null),
 (102, 2, 'Org RO',   null),
 (103, 2, 'LangEdit', null),
 (105, 2, 'Machines2',null);

-- -------------------------------------------------
-- base.rolepermission
-- -------------------------------------------------
INSERT INTO base.rolepermission (id, roleid, permissionnr, crud) VALUES
 (1, 1, 1, 'r'),
 (2, 1, 2, 'cru'),
 (3, 1, 3, 'crud'),
 (5, 1, 5, 'crud'),
 (11, 1, 6, 'r'),
 (12, 1, 7, 'r'),
 (13, 1, 8, 'r'),
 (14, 1, 101, 'r');

-- -------------------------------------------------
-- base.useraccrole
-- -------------------------------------------------
INSERT INTO base.useraccrole (id, useraccid, roleid, fromdate, todate) VALUES
 (10000, 111, 1, '2026-01-01', '2026-12-31');


-- -------------------------------------------------
--Used in Code Generation  (TEMPLATE)
-- -------------------------------------------------
INSERT INTO base.zcodetemplate (orgnr, code, descr) VALUES
 (1, 'T1', 'T1 Desc'),
 (1, 'T2', 'T2 Desc'),
 (1, 'T3', 'T3 Desc');


-- -------------------------------------------------
-- Fix identity sequences
-- -------------------------------------------------
SELECT setval(pg_get_serial_sequence('base.org', 'nr'),            (SELECT MAX(nr) FROM base.org));
SELECT setval(pg_get_serial_sequence('base.zzz', 'id'),            (SELECT MAX(id) FROM base.zzz));
SELECT setval(pg_get_serial_sequence('base.useracc', 'id'),        (SELECT MAX(id) FROM base.useracc));
SELECT setval(pg_get_serial_sequence('base.role', 'id'),           (SELECT MAX(id) FROM base.role));
SELECT setval(pg_get_serial_sequence('base.rolepermission', 'id'), (SELECT MAX(id) FROM base.rolepermission));
SELECT setval(pg_get_serial_sequence('base.useraccrole', 'id'),    (SELECT MAX(id) FROM base.useraccrole));

COMMIT;