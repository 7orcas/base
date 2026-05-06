

UPDATE zLabel SET Code = 'x' WHERE id=10057
UPDATE zLabel SET Code = 'Language' WHERE id=10984
UPDATE zLabel SET Code = 'Lang' WHERE id=10057

SELECT *
FROM zLabel
WHERE code like 'Lang'
OR code like 'Language'
