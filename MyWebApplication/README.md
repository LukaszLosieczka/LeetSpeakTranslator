## Initialize database

`docker compose up`

## Install `dotnet ef`

`dotnet tool install --global dotnet-ef`

## Apply migration

`dotnet ef migrations add migrationName`

## Update database

`dotnet ef database update`
