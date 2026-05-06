SELECT Id
      ,UserName,NormalizedUserName
      ,PhoneNumber,Lang
  FROM AspNetUsers


  --INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES (NEWID(), 'Planner', 'PLANNER')
  select * from AspNetRoles

  --DELETE FROM AspNetUserRoles
  --INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('b36d82ea-1882-4896-aa17-457802d05244', 'f5be7687-5702-4ea0-aa27-769317870d46')
  INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('b36d82ea-1882-4896-aa17-457802d05244', '85100D64-EE40-4121-BB93-3B8D310B7975')
  select * from AspNetUserRoles