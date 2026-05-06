
delete from base.rolePermission where roleid>100;delete from base.role where id>100;

SELECT [id]
      ,[orgNr]
      ,[code]
      ,[descr]
      ,[encoded]
      ,[updated]
      ,[isActive]
  FROM [blue].[base].[role]
  order by code

