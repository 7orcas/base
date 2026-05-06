# ProgresSQL
# PowerShell	 

#Need to run once:   Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned

chcp 65001 | Out-Null
$env:PGCLIENTENCODING="UTF8"

#NOTE label_path not working
psql -h localhost -d blue -U postgres `
  -v label_path=C:/src/f2f/db/Labels `
  -f _reload.sql	 