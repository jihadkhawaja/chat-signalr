# Xamarin Chat SignalR Server

![Xamarin Chat SignalR Icon](docs/icon.png)

# Requirements
- Visual Studio 2022 (dotnet 6.0)
- [mssql server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Required)
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

# In Progress
- Use a database instead of Json serializations ✔️
