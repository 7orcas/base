BEGIN TRANSACTION;

SET IDENTITY_INSERT base.langCode ON;
INSERT INTO base.langCode (id, code, descr) VALUES (1, 'en', 'English');
INSERT INTO base.langCode (id, code, descr) VALUES (2, 'de', 'Deutsch');
INSERT INTO base.langCode (id, code, descr) VALUES (3, 'es', 'Espanol');
SET IDENTITY_INSERT base.langCode OFF;


SET IDENTITY_INSERT base.langKey ON;
INSERT INTO base.langKey (id, code) VALUES (1, 'test1');
INSERT INTO base.langKey (id, code) VALUES (2, 'test2');
INSERT INTO base.langKey (id, code) VALUES (3, 'apiError');
SET IDENTITY_INSERT base.langKey OFF;


SET IDENTITY_INSERT base.langLabel ON;
INSERT INTO base.langLabel (id, langKeyId,langCode,code,tooltip) VALUES (1, 1,'en','TEST1 EN','TEST 1 TT EN'); --test1
INSERT INTO base.langLabel (id, langKeyId,langCode,code,tooltip) VALUES (2, 1,'de','TEST1 DE','TEST 1 TT DE'); --test1
INSERT INTO base.langLabel (id, langKeyId,langCode,code,tooltip) VALUES (3, 2,'en','TEST2 EN','TEST 2 TT EN'); --test1
INSERT INTO base.langLabel (id, langKeyId,langCode,code,tooltip) VALUES (4, 2,'de','TEST2 DE','TEST 2 TT DE'); --test1

INSERT INTO base.langLabel (id, langKeyId,langCode,code) VALUES (5, 3,'en','Oops, something went wrong'); --apiError

SET IDENTITY_INSERT base.langLabel OFF;

COMMIT;