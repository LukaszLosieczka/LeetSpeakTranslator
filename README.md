# Translator Web Application

## Running
### Docker
```
cd MyWebApplication

docker compose up --build
```
Running on `localhost:8080`
### Locally 
If you want to run the application locally using local database, you need to update file `MyWebApplication/appsettings.json` with the correct `Connection String`.
```
cd MyWebApplication

dotnet run
```
Running on `localhost:5236`
## Testing
```
cd MyWebApplication.Tests

dotnet test
```
