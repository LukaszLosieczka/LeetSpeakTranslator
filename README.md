# Translator Web Application

## Running
### Docker
```
cd MyWebApplication

docker compose up --build
```
### Locally 
If you want to run the application locally using local database, you need to update file `MyWebApplication/appsettings.json` with the correct `Connection String`.
```
cd MyWebApplication

dotnet run
```

## Testing
```
cd MyWebApplication.Tests

dotnet test
```
