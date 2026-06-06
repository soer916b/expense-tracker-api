# Expense Tracker API

A small learning project built with C#, ASP.NET Core Minimal API, EF Core, and SQLite.

The API tracks expenses with basic CRUD operations, query filtering and sorting, input validation, and a monthly summary endpoint.

## Purpose

This project was built to practice:

- C#
- ASP.NET Core Minimal APIs
- CRUD operations
- SQLite and EF Core
- input validation
- basic API design
- integration testing

## Features

- Create, read, update, and delete expenses
- Store data in SQLite
- Validate request bodies and query parameters
- Filter expenses by category and date range
- Sort expenses by date or amount
- Return a monthly expense summary with count and total amount
- Automated integration tests for the main API flows
- Manual API testing through a `.http` file

## Stack

- C#
- ASP.NET Core Minimal API
- EF Core
- SQLite
- OpenAPI
- xUnit

## Project structure

- `Program.cs`
  - App startup and route composition
- `models/`
  - `expense.cs`: the `Expense` entity
  - `ExpenseContext.cs`: the EF Core database context
- `Endpoints/`
  - `ExpenseEndpoints.cs`: all expense API endpoints
- `Validation/`
  - `ExpenseValidation.cs`: request and query validation
  - `ExpenseQuery.cs`: filtering and sorting helpers
  - `ExpenseCategories.cs`: permitted expense categories
- `Migrations/`
  - EF Core migration files for the SQLite schema
- `ExpenseTracker.Api.http`
  - Manual request examples
- `ExpenseTracker.Api.Tests/`
  - Integration tests for the API

## Expense model

```csharp
public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public DateOnly Date { get; set; }
}
```

## Endpoints

- `GET /`
- `GET /expenses`
- `GET /expenses/{id}`
- `POST /expenses`
- `PUT /expenses/{id}`
- `DELETE /expenses/{id}`
- `GET /expenses/summary?year={year}&month={month}`

## Run the project

```bash
dotnet restore
dotnet ef database update
dotnet run
```

## Run the tests

```bash
dotnet test
```

## Notes

- Migrations are included in the repository
- The SQLite database file is not committed
- The monthly summary endpoint performs aggregation in the database through EF Core
