BEGIN TRANSACTION;

SET IDENTITY_INSERT base.org ON;
INSERT INTO base.org (nr, code, descr) VALUES (0, 'Org Base', 'Base Organisation');
INSERT INTO base.org (nr, code, descr,langCode,langLabelVariant,encoded) VALUES (1, 'Org 1', 'Org 1 Description','en',1,'{Languages:[{LangCode:"en",IsEditable:true},{LangCode:"de",IsEditable:true},{LangCode:"c1",IsEditable:false},{LangCode:"c2",IsEditable:false}]}');
INSERT INTO base.org (nr, code, descr,langCode,langLabelVariant,encoded) VALUES (2, 'Org 2', 'Org 2 Description','en',2,'{Languages:[{LangCode:"de",IsEditable:true}]}');
INSERT INTO base.org (nr, code, descr,langCode) VALUES (3, 'Org 3', 'Org 3 Description','de');
SET IDENTITY_INSERT base.org OFF;


--{IsLangCodeEditable:false,Languages:["en","de","c1","c2"]}
--{IsLangCodeEditable:true,Languages:["de","c1","c2"]}
--{Languages:[{LangCode:"en",IsEditable:true},{LangCode:"de",IsEditable:true},{LangCode:"c1",IsEditable:false},{LangCode:"c2",IsEditable:false}]}
--{Languages:[{LangCode:"de",IsEditable:true}]}

SET IDENTITY_INSERT base.zzz ON;
INSERT INTO base.zzz (id, xxx, yyy) VALUES (1, '1', '1');
INSERT INTO base.zzz (id, xxx, yyy) VALUES (2, 'user', 'xx123');
SET IDENTITY_INSERT base.zzz OFF;

SET IDENTITY_INSERT base.userAcc ON;
INSERT INTO base.userAcc (id, zzzId, orgNr,langCode) VALUES (1, 1, 1,'de');
INSERT INTO base.userAcc (id, zzzId, orgNr,langCode) VALUES (2, 2, 1,'en');
INSERT INTO base.userAcc (id, zzzId, orgNr,langCode) VALUES (3, 2, 2,'de');
SET IDENTITY_INSERT base.userAcc OFF;

/*
SET IDENTITY_INSERT base.permission ON;
INSERT INTO base.permission (id, code, descr) VALUES (10,  'label',         'Language Labels');
INSERT INTO base.permission (id, code, descr) VALUES (20,  'profs',         'Professionals');
INSERT INTO base.permission (id, code, descr) VALUES (30,  'org',            'Organisations');
INSERT INTO base.permission (id, code, descr) VALUES (40,  'perms',       'Permissions');
INSERT INTO base.permission (id, code, descr) VALUES (50,  'role',            'User Roles');
INSERT INTO base.permission (id, code, descr) VALUES (60,  'user',           'Users');
INSERT INTO base.permission (id, code, descr) VALUES (70,  'ref',              'References');
INSERT INTO base.permission (id, code, descr) VALUES (80,  'machine',   'Machines');
INSERT INTO base.permission (id, code, descr) VALUES (90,  'audit',         'Audit');
--INSERT INTO base.permission (id, code, descr) VALUES (100,  'lang',         'Lang');
SET IDENTITY_INSERT base.permission OFF;
*/

SET IDENTITY_INSERT base.role ON;
INSERT INTO base.role (id, orgNr, code, descr) VALUES (1, 0, 'Admin', 'Full Admin Access');
INSERT INTO base.role (id, orgNr, code) VALUES (2, 0, 'Org RO');
INSERT INTO base.role (id, orgNr, code) VALUES (3, 0, 'LangEdit');
INSERT INTO base.role (id, orgNr, code) VALUES (4, 0, 'Machines0');
INSERT INTO base.role (id, orgNr, code) VALUES (5, 1, 'Machines1');
INSERT INTO base.role (id, orgNr, code) VALUES (6, 2, 'Machines2');

INSERT INTO base.role (id, orgNr, code, isActive) VALUES (7, 0, 'Role1', 0);
INSERT INTO base.role (id, orgNr, code, isActive) VALUES (8, 0, 'Role2', 0);
INSERT INTO base.role (id, orgNr, code) VALUES (9, 0, 'Role3');
INSERT INTO base.role (id, orgNr, code) VALUES (10, 0, 'Role4');
INSERT INTO base.role (id, orgNr, code) VALUES (11, 0, 'Role5');
INSERT INTO base.role (id, orgNr, code) VALUES (12, 0, 'Role6');
INSERT INTO base.role (id, orgNr, code) VALUES (13, 0, 'Role7');
INSERT INTO base.role (id, orgNr, code) VALUES (14, 0, 'Role8');
INSERT INTO base.role (id, orgNr, code) VALUES (15, 0, 'Role9');
INSERT INTO base.role (id, orgNr, code) VALUES (16, 1, 'Role10');
INSERT INTO base.role (id, orgNr, code) VALUES (17, 1, 'Role11');
INSERT INTO base.role (id, orgNr, code) VALUES (18, 1, 'Role12');
INSERT INTO base.role (id, orgNr, code) VALUES (19, 1, 'Role13');
INSERT INTO base.role (id, orgNr, code) VALUES (20, 1, 'Role14');
INSERT INTO base.role (id, orgNr, code) VALUES (21, 1, 'Role15');
INSERT INTO base.role (id, orgNr, code) VALUES (22, 1, 'Role16');
INSERT INTO base.role (id, orgNr, code) VALUES (23, 1, 'Role17');
INSERT INTO base.role (id, orgNr, code) VALUES (24, 1, 'Role18');
INSERT INTO base.role (id, orgNr, code) VALUES (25, 1, 'Role19');
INSERT INTO base.role (id, orgNr, code) VALUES (26, 1, 'Role20');
INSERT INTO base.role (id, orgNr, code) VALUES (27, 1, 'Role21');
INSERT INTO base.role (id, orgNr, code) VALUES (28, 1, 'Role22');
INSERT INTO base.role (id, orgNr, code) VALUES (29, 1, 'Role23');
INSERT INTO base.role (id, orgNr, code) VALUES (30, 1, 'Role24');

SET IDENTITY_INSERT base.role OFF;

SET IDENTITY_INSERT base.rolePermission ON;

INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (1, 1, 3, 'crud');
--INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (2, 1, 102, 'crud');
INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (3, 1, 1, 'r');
INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (4, 1, 2, 'cru');
INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (5, 1, 5, 'crud');
INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (6, 1, 6, 'crud');
--INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (7, 1, 'ref', 'crud');
INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (8, 1, 101, 'cd');
INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (9, 4, 101, 'd');
INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (10, 5, 101, 'ur');
INSERT INTO base.rolePermission (id, roleId, permissionNr, crud) VALUES (11, 6, 101, 'rd');
--INSERT INTO base.rolePermission (id, roleId, permissionNrId, crud) VALUES (13, 1, 100, 'r');

SET IDENTITY_INSERT base.rolePermission OFF;

SET IDENTITY_INSERT base.userAccRole ON;
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (1, 2, 1);
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (2, 2, 2);
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (3, 2, 3);
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (4, 2, 4);
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (5, 2, 5);
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (6, 2, 6);
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (7, 3, 1);
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (8, 3, 5);
INSERT INTO base.userAccRole (id, userAccId, roleId) VALUES (9, 3, 6);
SET IDENTITY_INSERT base.userAccRole OFF;


COMMIT;