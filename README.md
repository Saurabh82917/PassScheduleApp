# Pass Schedule App

ASP.NET Core MVC pass-schedule application. The project currently uses the SQL Server repository; an Oracle repository implementation is also included.

## Local setup

1. Install the .NET SDK matching the project target (`net9.0`).
2. Copy `PassScheduleApp/appsettings.example.json` to `PassScheduleApp/appsettings.json`.
3. Set `ConnectionStrings:DefaultConnection` to your database connection string. Keep this local settings file out of source control.
4. Run the application from the project directory:

   ```powershell
   dotnet run
   ```

To use Oracle, change the repository registration in `PassScheduleApp/Program.cs` to `OracleServerPassScheduleRepository` and configure an Oracle connection string locally.