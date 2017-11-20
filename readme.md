# RESTful Api
Just simple RESTful Api with token authentication.


## How to run
1. Clone this repo.
2. Create `appsettings.json` like
```json
{
  "MongoConnection": {
    "ConnectionString": "YOUR_MONGO_CONNTECTION_STRING",
    "DatabaseName": "YOUR_DATABASE_NAME"
  },
  "Token": {
    "Key": "YOUR_SECRETKEY",
    "Issuer": "ISSUER",
    "Audience": "AUDIENCE",
    "ExpiryMinutes": 60
  }
}
```
3. Run

