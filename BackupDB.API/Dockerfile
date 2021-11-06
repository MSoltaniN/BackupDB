FROM mcr.microsoft.com/dotnet/aspnet:2.1-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:2.1-focal AS build
WORKDIR /src
COPY ["BackupDB.API/BackupDB.API.csproj", "BackupDB.API/"]
RUN dotnet restore "BackupDB.API/BackupDB.API.csproj"
COPY . .
WORKDIR "/src/BackupDB.API"
RUN dotnet build "BackupDB.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackupDB.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackupDB.API.dll"]
