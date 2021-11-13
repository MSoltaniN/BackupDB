@echo OFF
cls
echo building SPA and  publish to wwwroot of API....
cd ..\..\BackupDB-SPA
ng build --configuration=production   
PAUSE
cls
echo  building API and publish to DeploymentFiles/IIS/publish...
cd ..\BackupDB.API
dotnet publish "BackupDB.API.csproj" -c Release -o ../DeploymentFiles/IIS/publish
dotnet ef database update 
PAUSE