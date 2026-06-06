using Microsoft.EntityFrameworkCore;

public static class ExpenseEndpoints
{
    public static IEndpointRouteBuilder MapExpenseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/expenses", GetExpenses)
            .WithName("GetExpenses");

        app.MapGet("/expenses/{id}", GetExpenseById)
            .WithName("GetExpenseById");

        app.MapPost("/expenses", CreateExpense)
            .WithName("CreateExpense");

        app.MapDelete("/expenses/{id}", DeleteExpense)
            .WithName("DeleteExpense");

        app.MapPut("/expenses/{id}", UpdateExpense)
            .WithName("UpdateExpense");

        app.MapGet("/expenses/summary", GetSummaryYearMonth)
            .WithName("GetSummaryYearMonth");

        return app;
    }

    private static async Task<IResult> GetExpenses(
        ExpenseContext db,
        string? category,
        DateOnly? startDate,
        DateOnly? endDate,
        string? sortBy,
        string? sortOrder)
    {
        var validationError = ExpenseValidation.ValidateExpenseQuery(
            category,
            startDate,
            endDate,
            sortBy,
            sortOrder);

        if (validationError != null)
        {
            return validationError;
        }

        var query = db.Expenses.AsQueryable();

        query = ExpenseQuery.ApplyFilters(query, category, startDate, endDate);
        query = ExpenseQuery.ApplySorting(query, sortBy, sortOrder);

        List<Expense> expenses = await query.ToListAsync();

        return Results.Ok(expenses);
    }

    private static async Task<IResult> GetExpenseById(ExpenseContext db, int id)
    {
        Expense? expense = await db.Expenses.FindAsync(id);

        return expense is null
            ? Results.NotFound()
            : Results.Ok(expense);
    }

    private static async Task<IResult> CreateExpense(ExpenseContext db, Expense expense)
    {
        var validationError = ExpenseValidation.ValidateExpense(expense);
        if (validationError != null)
        {
            return validationError;
        }

        await db.Expenses.AddAsync(expense);
        await db.SaveChangesAsync();

        return Results.Created($"/expenses/{expense.Id}", expense);
    }

    private static async Task<IResult> DeleteExpense(ExpenseContext db, int id)
    {
        Expense? expense = await db.Expenses.FindAsync(id);

        if (expense == null)
        {
            return Results.NotFound("Expense not found.");
        }

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync();

        return Results.Ok(expense);
    }

    private static async Task<IResult> UpdateExpense(ExpenseContext db, int id, Expense expense)
    {
        Expense? expenseToUpdate = await db.Expenses.FindAsync(id);
        if (expenseToUpdate == null)
        {
            return Results.NotFound("Expense not found.");
        }

        var validationError = ExpenseValidation.ValidateExpense(expense);
        if (validationError != null)
        {
            return validationError;
        }

        expenseToUpdate.Amount = expense.Amount;
        expenseToUpdate.Category = expense.Category;
        expenseToUpdate.Description = expense.Description;
        expenseToUpdate.Date = expense.Date;

        await db.SaveChangesAsync();

        return Results.Ok(expenseToUpdate);
    }

    private static async Task<IResult> GetSummaryYearMonth(ExpenseContext db, int year, int month)
    {
        var validationError = ExpenseValidation.ValidateSummaryInput(year, month);
        if (validationError != null)
        {
            return validationError;
        }

        var summaryQuery = db.Expenses
            .Where(expense => expense.Date.Year == year && expense.Date.Month == month);

        int countMonth = await summaryQuery.CountAsync();
        decimal amountMonth = countMonth == 0
            ? 0
            : await summaryQuery.SumAsync(expense => expense.Amount);

        var summary = new
        {
            Year = year,
            Month = month,
            ExpenseCount = countMonth,
            TotalAmount = amountMonth
        };

        return Results.Ok(summary);
    }
}
