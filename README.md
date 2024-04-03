
# <img src="logo/ogu-logo.png" alt="Header" width="24"/> Ogu.Dal 

| **Build Status** | **Ogu.Dal.Sql** | **Ogu.Dal.MongoDb** | **Ogu.Dal.Redis** | **Ogu.Dal.Abstractions** |
|-----------------|-----------------|---------------------|-------------------|--------------------------|
| [![.NET Core Desktop](https://github.com/ogulcanturan/Ogu.Dal/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/ogulcanturan/Ogu.Dal/actions/workflows/dotnet.yml) | [![NuGet](https://img.shields.io/nuget/v/Ogu.Dal.Sql.svg?color=1ecf18)](https://nuget.org/packages/Ogu.Dal.Sql) | [![NuGet](https://img.shields.io/nuget/v/Ogu.Dal.MongoDb.svg?color=1ecf18)](https://nuget.org/packages/Ogu.Dal.MongoDb) | [![NuGet](https://img.shields.io/nuget/v/Ogu.Dal.Redis.svg?color=1ecf18)](https://nuget.org/packages/Ogu.Dal.Redis) | [![NuGet](https://img.shields.io/nuget/v/Ogu.Dal.Abstractions.svg?color=1ecf18)](https://nuget.org/packages/Ogu.Dal.Abstractions) |
| | [![Nuget](https://img.shields.io/nuget/dt/Ogu.Dal.Sql.svg?logo=nuget)](https://nuget.org/packages/Ogu.Dal.Sql) | [![Nuget](https://img.shields.io/nuget/dt/Ogu.Dal.MongoDb.svg?logo=nuget)](https://nuget.org/packages/Ogu.Dal.MongoDb) | [![Nuget](https://img.shields.io/nuget/dt/Ogu.Dal.Redis.svg?logo=nuget)](https://nuget.org/packages/Ogu.Dal.Redis) | [![Nuget](https://img.shields.io/nuget/dt/Ogu.Dal.Abstractions.svg?logo=nuget)](https://nuget.org/packages/Ogu.Dal.Abstractions) |

# Ogu.Dal.Sql

A wrapper built using Entity Framework Core for accessing SQL Server databases. It provides repositories with structured data retrieval logic, including pagination, Unit of Work pattern, and observer (WITH (NOLOCK)) specific to SQL Server. [More info](https://github.com/ogulcanturan/Ogu.Dal/tree/master/src/Ogu.Dal.Sql#readme)

## Installation

You can install the library via NuGet Package Manager:

```bash
dotnet add package Ogu.Dal.Sql
```

## Sample Application
A sample application demonstrating the usage of Ogu.Dal.Sql can be found [here](https://github.com/ogulcanturan/Ogu.Dal/tree/master/samples/Sql.Sample.Api).

# Ogu.Dal.MongoDb

A C# MongoDB driver wrapper. It includes repositories with pagination logic and method names similar to SQL repository names, making it easy to retrieve data using LINQ. Additionally, it provides attributes for specifying database and collection names, along with MongoIndexAttribute for creating indexes. [More info](https://github.com/ogulcanturan/Ogu.Dal/tree/master/src/Ogu.Dal.MongoDb#readme)

## Installation

You can install the library via NuGet Package Manager:

```bash
dotnet add package Ogu.Dal.MongoDb
```

## Sample Application
A sample application demonstrating the usage of Ogu.Dal.MongoDb can be found [here](https://github.com/ogulcanturan/Ogu.Dal/tree/master/samples/MongoDb.Sample.Api).

# Ogu.Dal.Redis

A StackExchange.Redis wrapper. It contains RedisContext for executing queries safely. The library is currently in progress, and more features will be added in the future. [More info](https://github.com/ogulcanturan/Ogu.Dal/tree/master/src/Ogu.Dal.Redis#readme)

## Installation

You can install the library via NuGet Package Manager:

```bash
dotnet add package Ogu.Dal.Redis
```

## Sample Application
A sample application demonstrating the usage of Ogu.Dal.Redis is not exist at this moment.



