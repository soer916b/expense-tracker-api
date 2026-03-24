using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/**************************************/

// Add services to the container.
builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

builder.Services.AddDbContext<ExpenseContext>(options =>
    options.UseSqlite(connectionString));

/**************************************/

var app = builder.Build();

/**************************************/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

/**************************************/

static IResult? ValidateExpense (Expense expense)
{
    if (expense.Amount <= 0)
    {
        return Results.BadRequest("Amount must be greater than 0.");
    }
    if (string.IsNullOrWhiteSpace(expense.Category))
    {
        return Results.BadRequest("Category must not be blank.");
    }
    if (expense.Description.Length > 200)
    {
        return Results.BadRequest("Description must not be longer than 200 characters.");
    }
    if (expense.Date == default)
    {
        return Results.BadRequest("Date must be set");
    }
    return null;
}

/**************************************/

app.MapGet("/", () => "Welcome to the Expense Tracker!");

/**************************************/

app.MapGet("/expenses", async (ExpenseContext db) =>
{
    List <Expense> result = await db.Expenses.ToListAsync();
    return Results.Ok(result);
})
.WithName("GetExpenses");

/**************************************/

app.MapGet("/expenses/{id}", async (ExpenseContext db, int id) =>
{
    Expense? expense = await db.Expenses.FindAsync(id);
    if (expense == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(expense);
})
.WithName("GetExpenseById");

/**************************************/

app.MapPost("/expenses", async (ExpenseContext db, Expense expense) =>
{
    var validationError = ValidateExpense(expense);
    if (validationError != null)
    {
        return validationError;
    }

    await db.Expenses.AddAsync(expense);
    await db.SaveChangesAsync();

    return Results.Created($"/expenses/{expense.Id}", expense);
})
.WithName("CreateExpense");

/**************************************/

app.MapDelete("/expenses/{id}", async (ExpenseContext db, int id) =>
{
    Expense? expense = await db.Expenses.FindAsync(id);
    if (expense == null)
    {
        return Results.NotFound();
    }
    db.Expenses.Remove(expense);
    await db.SaveChangesAsync();
    return Results.Ok(expense);
})
.WithName("DeleteExpense");

/**************************************/

app.MapPut("/expenses/{id}", async (ExpenseContext db, int id, Expense expense) =>
{
    var validationError = ValidateExpense(expense);
    if (validationError != null)
    {
        return validationError;
    }

    Expense? expenseToUpdate = await db.Expenses.FindAsync(id);
    if (expenseToUpdate == null)
    {
        return Results.NotFound();
    }
    expenseToUpdate.Amount = expense.Amount;
    expenseToUpdate.Category = expense.Category;
    expenseToUpdate.Description = expense.Description;
    expenseToUpdate.Date = expense.Date;
    await db.SaveChangesAsync();
    return Results.Ok(expenseToUpdate);
})
.WithName("UpdateExpense");

/**************************************/

app.Run();
