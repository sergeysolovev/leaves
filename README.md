# Leaves API

ASP.NET Core 2.0 API and MVC client for applying and managing leaves of absence. Authenticates through [Google Open Id Connect](https://developers.google.com/identity/protocols/OpenIDConnect), uses [hybrid server-side flow](https://developers.google.com/identity/sign-in/web/server-side-flow) and [Google Calendar API](https://developers.google.com/google-apps/calendar/) to share an approved leave when the user is offline.

Live version of the API is available here: [https://leaves-api.now.sh/api/](https://leaves-api.now.sh/api).

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

Install [.NET Core 2.0.0 Preview 2](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.0-preview2-download.md) to build and run the apps.

### Building and running API
Build and create the database
```
cd src/api

# Build
dotnet build

# Create the database
dotnet ef database update
```

Run on port 8080 and optionally specify comma-separated list of admins
```
# UNIX
ASPNETCORE_URLS="http://*:8080" dotnet run --admins=<your@email.com>

# Windows PowerShell
$env:ASPNETCORE_URLS="http://*:8080"; dotnet run --admins=<your@email.com>

# Windows CMD
SET ASPNETCORE_URLS=http://*:8080 && dotnet run --admins=<your@email.com>
```

Make a test request
```
# UNIX
curl -i http://localhost:8080/leaves
```
It should return `401 Unauthorized`, since ID token is not provided. To identify the request, run the client (see below) and open <http://localhost:8081/auth/idtoken> in browser to get the token.
```
# UNIX
curl -v http://localhost:8080/leaves -H "Authorization: Bearer <your id token>"
```

### Building and running the client
Build
```
cd src/clients/aspnetcore

# Build
dotnet build
```

Check out the API base url in appsettings.json
```
"LeavesApi": {
  "BaseUrl": "http://localhost:8080/"
},
```
Change it if you use another url for the API.

Run on port 8081
```
# UNIX
ASPNETCORE_URLS="http://*:8081" dotnet run

# Windows PowerShell
$env:ASPNETCORE_URLS="http://*:8081"; dotnet run

# Windows CMD
SET ASPNETCORE_URLS=http://*:8081 && dotnet run
```

To test, open <http://localhost:8081> in browser.

### Use Visual Studio Code to run and debug the apps
Just open the API or the client in vscode and start debugging.

To set your admin account, open `src/api/.vscode/launch.json` and edit the following line:
```
"args": ["--admins=<your@email.com>"],
```

## API Deployment

The API can be deployed with Docker, see [Dockerfile](https://github.com/sergeysolovev/leaves-api/blob/master/src/api/Dockerfile).

### Create production build

First, build the API and create/update the database with [above instructions](#building-and-running-api).

Next, create self-contained production build
```
cd src/api

# Create production build
dotnet publish -c Debug -r linux-x64
```
The output files as well as `leaves.db` and `Dockerfile` can be found in `src/api/bin/Debug/netcoreapp2.0/linux-x64/publish`.

### Docker image

Build leaves-api docker image
```
docker build -t leaves-api --build-arg source=bin/Debug/netcoreapp2.0/linux-x64/publish .
```

Run a local docker container to test the build
```
docker run --rm -it -p 8080:80 leaves-api
```

### Deploy on now.sh

Note: Since the build contains sensitive data, such as refresh tokens and files, larger than 1Mb,  [now.sh OSS free plan](https://zeit.co/pricing) is not appropriate for the API deployment.

Install now CLI
```
npm install -g now
```

There's no need to build the docker image since now.sh does it on it's side. Create [the production build](#create-production-build) and run `now`
```
now bin/Debug/netcoreapp2.0/linux-x64/publish
```
This will copy output files to the cloud and run the docker container.

## Client Deployment

The client hasn't dockerized yet.

### Create production build

Create a production build
```
cd src/clients/aspnetcore

# Create production build
dotnet publish -c Debug
```
The output files can be found in `src/clients/aspnetcore/bin/Debug/netcoreapp2.0/publish`.

## Built With

* [ASP.NET Core 2.0](https://github.com/aspnet/Home)
* [Entity Framework Core 2.0](https://github.com/aspnet/EntityFramework)
* [Identity 2.0](https://github.com/aspnet/Identity)
* [SQLite 3](https://www.sqlite.org)

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

* **Sergey Solovev** - [sergeysolovev](https://github.com/sergeysolovev)
* **Max Ermolaev** - [lainiwakurafan](https://github.com/lainiwakurafan)

See also the list of [contributors](https://github.com/sergeysolovev/leaves-api/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
