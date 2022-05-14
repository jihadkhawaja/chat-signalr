# Xamarin Chat SignalR Server

![Xamarin Chat SignalR Icon](docs/icon.png)

|:warning: WARNING|
|:---------------------------|
|Don't use this branch for production|

# Requirements
- dotnet 6.0 (Required), you can use [Visual Studio 2022](https://visualstudio.microsoft.com/vs/preview/) or install [dotnet-6.0 sdk and runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and use your favorite editor like [Visual Studio Code](https://code.visualstudio.com/)
- [Microsoft SQL Server (mssql)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or your preferred database engine (Required)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (optinal), You can use visual studio built-in sql server browser and connect your database.

# Usage
- Create appsecrets.json file and add your own Jwt like below, include it in project solution to be created on build.
```
{
  "Secrets": {
    "Jwt": "cACLPY7=*Pm5C%?3"
  }
}
```
- Set your database connection strings in appsettings.json
```
"ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=mobilechatdb;Trusted_Connection=True;",
    "ProductionConnection": "Server=localhost;Database=databasename;User Id=SA;Password=password;"
  }
```
- Test your database connection, in the Package Manager Console (Ctrl+`)
```
Add-Migration [your migration name]
Update-Database
```
If your database is setup correctly you should find the database along with your models tables added to it.

#### :grey_exclamation: Notice
This project is under heavy refactoring and development, You may contribute once a release is published.
