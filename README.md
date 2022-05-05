# Xamarin Chat SignalR Server

![Xamarin Chat SignalR Icon](docs/icon.png)

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
