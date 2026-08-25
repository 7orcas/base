\set ON_ERROR_STOP on
BEGIN;


-- -------------------------------------------------
-- cntrl.loginoption
-- -------------------------------------------------
INSERT INTO cntrl.loginoption (urlsuffix, orgnr, orgnrs, langcode, langlabelvariant, langcodes, isdefault)
VALUES ('blue', 1, '1', 'en', 0, 'en', true);

INSERT INTO cntrl.loginoption (urlsuffix, orgnr, orgnrs, langcode, langlabelvariant, langcodes, ismasqueradeenabled)
VALUES ('service', 1, '1', 'en', 0, 'en', true);

INSERT INTO cntrl.loginoption (urlsuffix, orgnr, orgnrs, langcode, langlabelvariant, langcodes, isdefault)
VALUES ('admin', 1, '1,2', 'en', 0, 'en,de,it,es,ma,cn', false);

INSERT INTO cntrl.loginoption (urlsuffix, orgnr, orgnrs, langcode, langlabelvariant, langcodes, ismasqueradeenabled)
VALUES ('all', 2, '1,2,3', 'de', 1, 'en,de,es,xx', true);

INSERT INTO cntrl.loginoption (urlsuffix, orgnr, orgnrs, langcode, langlabelvariant, langcodes, successaction)
VALUES ('api', 1, '1', 'en', 0, 'en', 1);

INSERT INTO cntrl.loginoption (urlsuffix, orgnr, orgnrs, langcode, langlabelvariant, langcodes, successaction)
VALUES ('3', 3, '3', 'en', 0, 'en', 1);

-- -------------------------------------------------
-- base.org
-- -------------------------------------------------
INSERT INTO base.org (nr, code, descr, langcode, langlabelvariant)
VALUES (0, 'Org Base', 'Base Organisation', 'en', 0);

INSERT INTO base.org (nr, code, descr, langcode, langlabelvariant, mfa, 
	apikey,
    ispasswordresetenabled, issignupenabled, isemailrequired, isemailverified, ismasqueradeenabled,
	icon, 
	encoded)
VALUES
(1, 'Blue', 'Focus your heart', 'en', 1, 0, 
  'X123',	
  true, true, true, true, true, 
  '/images/Icons/BlueTransparentBlack512.png',
 '{Languages:[{LangCode:"en",IsEditable:true},{LangCode:"de",IsEditable:true},{LangCode:"c1",IsEditable:false},{LangCode:"c2",IsEditable:false}]}');

INSERT INTO base.org (nr, code, descr, langcode, langlabelvariant, icon, encoded)
VALUES
(2, 'Org 2', 'Org 2 Description', 'en', 2, 
 '/images/Icons/BlueBlack512.png',
 '{Languages:[{LangCode:"de",IsEditable:true}]}');

INSERT INTO base.org (nr, code, descr, langcode, langlabelvariant, encoded)
VALUES (3, 'Org 3', 'Org 3 Description', 'de', 1,
'{Languages:[{LangCode:"de",IsEditable:true}]}');


-- -------------------------------------------------
-- base.zzz
-- -------------------------------------------------
INSERT INTO base.zzz (id, xxx, yyy, email, orgnrdefault, langcode, isemailverified, ismfarequired)
VALUES
(1, '1', '1.bElHDRD8r/YSi60KfZ895Q==.OtZFpeplYRtPbhp1iDJv81XGo1hcP9okCCcDMK1l/ZQ=', --1
 'john.stewart@7orcas.com', 1, 'en', true, false);
INSERT INTO base.zzz (id, xxx, yyy, email, orgnrdefault, langcode)
VALUES
 (2, 'user', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (-1, '$', '1.8da2kjhE7Yjh8LpInlivMQ==.jRwSyq64a4AYu6mSxBYaZ3tWgyRhPD2ZJlzHovwBYr0=', 'js@7orcas.com', 1, 'en'),
 (11, 'user11', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (12, 'user12', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (13, 'user13', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (14, 'user14', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (15, 'user15', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (16, 'user16', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (17, 'user17', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (18, 'user18', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (19, 'user19', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (21, 'user21', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (22, 'user22', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (23, 'user23', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (24, 'user24', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (25, 'user25', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (26, 'user26', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (27, 'user27', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (28, 'user28', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (29, 'user29', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (31, 'user31', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (32, 'user32', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (33, 'user33', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (34, 'user34', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (35, 'user35', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (36, 'user36', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (37, 'user37', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (38, 'user38', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (39, 'user39', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (41, 'user41', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (42, 'user42', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (43, 'user43', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (44, 'user44', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (45, 'user45', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (46, 'user46', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (47, 'user47', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (48, 'user48', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en'),
 (49, 'user49', '1.VJXOTS5/KLiukWt4kMACAw==.KUeftBqtDfYlkZ8D4qNIAGm4g3cdoVB+67e5bIO8qRI=', 'xx123', 1, 'en');

 
-- -------------------------------------------------
-- base.useracc
-- -------------------------------------------------
INSERT INTO base.useracc (id, zzzid, orgnr) VALUES
 (1, 1, 1);
INSERT INTO base.useracc (id, zzzid, orgnr) VALUES
 (11, 1, 2),
 (2, 2, 1),
 (3, 2, 2),
 (111, 11, 1),
 (112, 12, 1),
 (113, 13, 1),
 (114, 14, 1),
 (115, 15, 1), 
 (116, 16, 1),
 (117, 17, 1),
 (118, 18, 1),
 (119, 19, 1),
 (121, 21, 1), 
 (122, 22, 1),
 (123, 23, 1),
 (124, 24, 1),
 (125, 25, 1),
 (126, 26, 1), 
 (127, 27, 1),
 (128, 28, 1),
 (129, 29, 1),
 (131, 31, 1),
 (132, 32, 1), 
 (133, 33, 1),
 (134, 34, 1),
 (135, 35, 1),
 (136, 36, 1),
 (137, 37, 1), 
 (138, 38, 1),
 (139, 39, 1),
 (141, 41, 1),
 (142, 42, 1),
 (143, 43, 1),  
 (144, 44, 1),  
 (145, 45, 1),  
 (146, 46, 1),  
 (147, 47, 1),  
 (148, 48, 1),  
 (149, 49, 1);

-- -------------------------------------------------
-- base.role
-- -------------------------------------------------
INSERT INTO base.role (id, orgnr, code, descr)
VALUES
(1, 0, 'Admin', 'Full Admin Access');

INSERT INTO base.role (id, orgnr, code)
VALUES
(2, 0, 'Org RO'),
(3, 0, 'LangEdit'),
(4, 0, 'Machines0'),
(5, 1, 'Machines1'),
(6, 2, 'Machines2');

INSERT INTO base.role (id, orgnr, code, isactive)
VALUES
(7, 0, 'Role1', FALSE),
(8, 0, 'Role2', FALSE);

INSERT INTO base.role (id, orgnr, code)
VALUES
(9, 0, 'Role3'),
(10, 0, 'Role4'),
(11, 0, 'Role5'),
(12, 0, 'Role6'),
(13, 0, 'Role7'),
(14, 0, 'Role8'),
(15, 0, 'Role9'),
(16, 1, 'Role10'),
(17, 1, 'Role11'),
(18, 1, 'Role12'),
(19, 1, 'Role13'),
(20, 1, 'Role14'),
(21, 1, 'Role15'),
(22, 1, 'Role16'),
(23, 1, 'Role17'),
(24, 1, 'Role18'),
(25, 1, 'Role19'),
(26, 1, 'Role20'),
(27, 1, 'Role21'),
(28, 1, 'Role22'),
(29, 1, 'Role23'),
(30, 1, 'Role24');

-- -------------------------------------------------
-- base.rolepermission
-- -------------------------------------------------
INSERT INTO base.rolepermission (id, roleid, permissionnr, crud)
VALUES
(1, 1, 3, 'crud'),
(3, 1, 1, 'r'),
(4, 1, 2, 'cru'),
(5, 1, 5, 'crud'),
--(6, 1, 6, 'crud'), --Users
(8, 1, 101, 'cd'),
(9, 4, 101, 'd'),
(10, 5, 101, 'ur'),
(11, 6, 101, 'rd');

-- -------------------------------------------------
-- base.useraccrole
-- -------------------------------------------------
INSERT INTO base.useraccrole (id, useraccid, roleid, fromdate, todate)
VALUES
(1, 2, 1, '2026-01-01', '2026-12-31');

INSERT INTO base.useraccrole (id, useraccid, roleid)
VALUES
(2, 2, 2),
(3, 2, 3),
(4, 2, 4),
(5, 2, 5),
(6, 2, 6),
(7, 3, 1),
(8, 3, 5),
(9, 3, 6),

(10, 1, 1),
(12, 1, 2),
(13, 1, 3),
(14, 1, 4),
(15, 1, 5),
(16, 1, 6);

-- -------------------------------------------------
-- Fix identity sequences
-- -------------------------------------------------
SELECT setval(pg_get_serial_sequence('base.org', 'nr'),
              (SELECT MAX(nr) FROM base.org));

SELECT setval(pg_get_serial_sequence('base.zzz', 'id'),
              (SELECT MAX(id) FROM base.zzz));

SELECT setval(pg_get_serial_sequence('base.useracc', 'id'),
              (SELECT MAX(id) FROM base.useracc));

SELECT setval(pg_get_serial_sequence('base.role', 'id'),
              (SELECT MAX(id) FROM base.role));

SELECT setval(pg_get_serial_sequence('base.rolepermission', 'id'),
              (SELECT MAX(id) FROM base.rolepermission));

SELECT setval(pg_get_serial_sequence('base.useraccrole', 'id'),
              (SELECT MAX(id) FROM base.useraccrole));

COMMIT;