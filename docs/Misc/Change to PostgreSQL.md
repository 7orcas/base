
Download and install Postgres
- cd [source files]\f2f\db\Postgres
- run 0_createDB.sql (in pgadmin)
- run _reload.ps1

Changing to Postgres in Backend Project
- Added package PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.*" />
- Removed <PackageReference Include="Microsoft.Data.SqlClient" Version="6.0.1" />
- Replace using Microsoft.Data.SqlClient; with using Npgsql;