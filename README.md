Download and install Visual Studio 2026
- Connect to GitHub account

Download and install GitHub
- https://git-scm.com/downloads
- On GitHub get the URL (https://github.com/7orcas/f2f.git)
- In terminal, git clone url

Download and install Postgres
- cd [source files]\f2f\db\Postgres
- run 0_createDB.sql (in pgadmin)
- run _reload.ps1



Docker
- https://www.docker.com/products/docker-desktop
- download: Windows AMD64 and run
- AI query
  - Give me step by step instructions to
    - Download and Install Docker on my Windows PC
	- Use a docker volume on my PC
	- Configure it to run Seq log view
	- Configure it to run Postgres
	- Make sure logs are stored locally
	- Make sure db is stored locally
	- Connect my c# .Net API program to connect to Postgres db
- Windows Subsystem for Linux will be installed (you may need to update - this will be highlighted)
- Run in terminal in solution root: docker compose up -d  (check: docker ps)


Swagger
- Need to login to get a token
- There is a PathBase so that needs to be configured in program.cs

Logging
- Use Serilog
- Enable json formatting
- Use log view Seq running in Docker




Running the Application
- Config startup projects:
  - Backend: start
  - FrontendServer: start
  - FrontendLogin: start
- https://localhost:7289  (as configured in FrontendLogin/Properties/launchsettings.json



