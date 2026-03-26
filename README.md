# Expense Tracker API V1

A small learning project built with C#, ASP.NET Core Minimal API, EF Core and SQLite.

The project implements a simple REST-style API for tracking expenses, with CRUD functionality, input validation and a monthly summary endpoint.  

The purpose of the project was to practice:
- C#
- ASP.NET Core minimal APIs
- CRUD operations
- SQLite and EF Core
- input validation
- basic API design

The project is kept small and focused as a V1 learning project.

## Features

- Create, read, update, and delete expenses
- Store data in SQLite
- Validate input for create and update
- Return clear HTTP responses
- Monthly summary endpoint with expense count and total amount
- Manual testing through a `.http` file

## Stack

- C#
- ASP.NET Core Web API
- EF Core
- SQLite

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
- GET /  
- GET /expenses  
- GET /expenses/{id}  
- POST /expenses  
- PUT /expenses/{id}  
- DELETE /expenses/{id}  
- GET /expenses/summary?year={year}&month={month}

## Validation

### Expense validation:

- amount must be greater than 0
- category must not be blank
- description must be 200 characters or less
- date must be set

### Summary validation:

- year must be positive
- month must be between 1 and 12

## Run the project
```bash
dotnet restore
dotnet ef database update
dotnet run
```
Then test the API using the included `.http`-file.

## Notes
- Migrations are included in the repository
- The SQLite database file is not committed
