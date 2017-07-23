# Leaves API

ASP.NET Core 2.0 API and MVC client for applying and managing leaves of absence. Authenticates through [Google Open Id Connect](https://developers.google.com/identity/protocols/OpenIDConnect), uses [hybrid server-side flow](https://developers.google.com/identity/sign-in/web/server-side-flow) and [Google Calendar API](https://developers.google.com/google-apps/calendar/) to share an approved leave when the user is offline.

## Live demo

* <https://leaves-api.now.sh> - the API
* <https://leaves.now.sh> - the client

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

Install [.NET Core 2.0.0 Preview 2](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.0-preview2-download.md) to build and run the apps.

Clone this repository and switch to its folder
```
git clone https://github.com/sergeysolovev/leaves.git
cd leaves
```

### Building and running the API
Build and create the database:
```
cd src/api
dotnet build
dotnet ef database update
```

Run locally and optionally specify comma-separated list of admins:
```
# UNIX
ASPNETCORE_URLS="http://*:8080" ASPNETCORE_ENVIRONMENT="Development" dotnet run --admins=<your@email.com>

# Windows PowerShell
$Env:ASPNETCORE_URLS = "http://*:8080"; $Env:ASPNETCORE_ENVIRONMENT = "Development"; dotnet run --admins=<your@email.com>
```

Make a test request:
```
# UNIX
curl -i http://localhost:8080/leaves
```
It should return `401 Unauthorized`, since ID token is not provided.

### Identifying requests to the API

To obtain the token, either:
* Open <https://leaves.now.sh/idtoken>, authenticate and copy it from there; or
* [Build and run](#building-and-running-the-client) the client locally and do the same: <http://localhost:8081/idtoken>.

Request examples:
```
# UNIX
curl -v http://localhost:8080/leaves -H "Authorization: Bearer <your id token>"
curl -v https://leaves-api.now.sh/leaves -H "Authorization: Bearer <your id token>"
```

### Building and running the client

Build:
```
cd src/clients/aspnetcore
dotnet build
```

Check out the API base url in [appsettings.json](https://github.com/sergeysolovev/leaves/blob/master/src/clients/aspnetcore/appsettings.json):
```
"LeavesApi": {
  "BaseUrl": "http://localhost:8080/"
},
```
Change it if you use another url for the API.

Run locally:
```
# UNIX
ASPNETCORE_URLS="http://*:8081" ASPNETCORE_ENVIRONMENT="Development" dotnet run

# Windows PowerShell
$Env:ASPNETCORE_URLS = "http://*:8081"; $Env:ASPNETCORE_ENVIRONMENT = "Development"; dotnet run
```

To test, open <http://localhost:8081> in the browser.

### Using Visual Studio Code to run and debug the apps
Just open the API or the client in vscode and start debugging.

To set your admin account, open `src/api/.vscode/launch.json` and edit the following line:
```
"args": ["--admins=<your@email.com>"],
```

## Deployment

To deploy the apps, it is suggested to create and dockerize a self-contained production build for each one.

### Creating production build

To create a production build for the API, make sure that the database file `src/api/leaves.db` is present and up-to-date. It is copied to the output folder during the build process.
```
cd src/api
dotnet ef database update
```

Create the build:
```
# the API
cd src/api
dotnet publish -r linux-x64

# the client
cd src/clients/aspnetcore
dotnet publish -r linux-x64
```
The output files can be found in `bin/Debug/netcoreapp2.0/linux-x64/publish`

### Dockerizing

Both the API and the client copy Dockerfile to production build output folder:
* [Dockerfile](https://github.com/sergeysolovev/leaves/blob/master/src/api/Dockerfile) - the API
* [Dockerfile](https://github.com/sergeysolovev/leaves/blob/master/src/clients/aspnetcore/Dockerfile) - the client

Install [docker](https://www.docker.com/) to build the images and run locally. Build:
```
# the API
cd src/api
docker build bin/Debug/netcoreapp2.0/linux-x64/publish -t leaves-api

# the client
cd src/clients/aspnetcore
docker build bin/Debug/netcoreapp2.0/linux-x64/publish -t leaves-client
```

Run the containers:
```
# the API
docker run --rm -it -p 8080:80 leaves-api

# the client
docker run --rm -it -p 8081:80 leaves-client
```

### Example deployment: now.sh

Since the API build contains sensitive data, such as refresh tokens and both builds contain files, larger than 1 Mb, [now.sh OSS free plan](https://zeit.co/pricing) won't work and is not appropriate.

To install now CLI:
```
npm install -g now
```

To deploy to [now.sh](https://now.sh) there's no need to build the docker images locally, since it is done in the cloud.
```
# the API
cd src/api
dotnet publish -r linux-x64
now bin/Debug/netcoreapp2.0/linux-x64/publish

# the client
cd src/clients/aspnetcore
dotnet publish -r linux-x64
now bin/Debug/netcoreapp2.0/linux-x64/publish
```
This will copy output files to the cloud and run the docker container in there.

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

See also the list of [contributors](https://github.com/sergeysolovev/leaves/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
