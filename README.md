# Local Development & Setup

## Prerequisites

- .NET 8 SDK

Download the SDK:

https://dotnet.microsoft.com/download/dotnet/8.0

## Clone the Repository

```bash
git clone https://github.com/DaniilHed/GenerativeParser.git
cd GenerativeParser
```

## Restore Dependencies

```bash
dotnet restore
```

## Run the Application

```bash
dotnet run --project GenerativeParser/GenerativeParser.csproj
```

After the application starts, open the Swagger UI using the URL displayed in the console, for example:

```
https://localhost:7157/swagger
```

> **Note:** The port may vary depending on your local launch profile.

## Run Tests

Run all unit tests:

```bash
dotnet test
```

To display detailed test output:

```bash
dotnet test --logger "console;verbosity=detailed"
```
