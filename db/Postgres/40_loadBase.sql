\set ON_ERROR_STOP on
BEGIN;

-- -------------------------------------------------
-- base.org
-- -------------------------------------------------
INSERT INTO base.org (nr, code, descr)
VALUES (0, 'Org Base', 'Base Organisation');

INSERT INTO base.org (nr, code, descr, langcode, langlabelvariant, encoded)
VALUES
(1, 'Org 1', 'Org 1 Description', 'en', 1,
 '{Languages:[{LangCode:"en",IsEditable:true},{LangCode:"de",IsEditable:true},{LangCode:"c1",IsEditable:false},{LangCode:"c2",IsEditable:false}]}');

INSERT INTO base.org (nr, code, descr, langcode, langlabelvariant, encoded)
VALUES
(2, 'Org 2', 'Org 2 Description', 'en', 2,
 '{Languages:[{LangCode:"de",IsEditable:true}]}');

INSERT INTO base.org (nr, code, descr, langcode, langlabelvariant, encoded)
VALUES (3, 'Org 3', 'Org 3 Description', 'de', 1,
'{Languages:[{LangCode:"de",IsEditable:true}]}');


-- -------------------------------------------------
-- base.zzz
-- -------------------------------------------------
INSERT INTO base.zzz (id, xxx, yyy)
VALUES
(1, '1', '1'),
(2, 'user', 'xx123');

-- -------------------------------------------------
-- base.useracc
-- -------------------------------------------------
INSERT INTO base.useracc (id, zzzid, orgnr, langcode)
VALUES
(1, 1, 1, 'de'),
(2, 2, 1, 'en'),
(3, 2, 2, 'de');

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
(6, 1, 6, 'crud'),
(8, 1, 101, 'cd'),
(9, 4, 101, 'd'),
(10, 5, 101, 'ur'),
(11, 6, 101, 'rd');

-- -------------------------------------------------
-- base.useraccrole
-- -------------------------------------------------
INSERT INTO base.useraccrole (id, useraccid, roleid)
VALUES
(1, 2, 1),
(2, 2, 2),
(3, 2, 3),
(4, 2, 4),
(5, 2, 5),
(6, 2, 6),
(7, 3, 1),
(8, 3, 5),
(9, 3, 6);

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