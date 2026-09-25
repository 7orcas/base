SELECT a.id,entitytypenr,crud,details
FROM base.audit a
LEFT JOIN base.useracc ua ON ua.id = a.useraccid
LEFT JOIN base.zzz z ON z.id = ua.zzzid
WHERE created > '2026-09-17';

--delete from base.audit where created > '2026-09-17'