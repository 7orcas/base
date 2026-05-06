REM ProgresSQL
REM CMD

psql -h localhost -d blue -U postgres ^
 -v label_pathX=C:/src/f2f/db/Labels ^
 -f _reload.sql 
	 