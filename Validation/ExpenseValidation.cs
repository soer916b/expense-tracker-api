public static class ExpenseValidation
{
    public static IResult? ValidateExpense(Expense expense)
    {
        if (expense.Amount <= 0)
        {
            return Results.BadRequest("Amount must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(expense.Category))
        {
            return Results.BadRequest("Category must not be blank.");
        }

        if (!ExpenseCategories.Permitted.Contains(expense.Category))
        {
            return Results.BadRequest(
                $"Category must be set to one of the following: {string.Join(", ", ExpenseCategories.Permitted)}");
        }

        if (expense.Description.Length > 200)
        {
            return Results.BadRequest("Description must not be longer than 200 characters.");
        }

        if (expense.Date == default)
        {
            return Results.BadRequest("Date must be set.");
        }

        return null;
    }

    public static IResult? ValidateExpenseQuery(
        string? category,
        DateOnly? startDate,
        DateOnly? endDate,
        string? sortBy,
        string? sortOrder)
    {
        if (category != null && string.IsNullOrWhiteSpace(category))
        {
            return Results.BadRequest("Category must not be blank.");
        }

        if (category != null && !ExpenseCategories.Permitted.Contains(category))
        {
            return Results.BadRequest(
                $"Category must be set to one of the following: {string.Join(", ", ExpenseCategories.Permitted)}");
        }

        if (startDate != null && endDate != null && startDate > endDate)
        {
            return Results.BadRequest("Start-date must not be greater than End-date.");
        }

        if (sortBy == null && sortOrder != null)
        {
            return Results.BadRequest("sortOrder requires sortBy to be set.");
        }

        if (sortBy != null && sortBy != "date" && sortBy != "amount")
        {
            return Results.BadRequest("sortBy must be either [date] or [amount]");
        }

        if (sortOrder != null && sortOrder != "asc" && sortOrder != "desc")
        {
            return Results.BadRequest("sortOrder must be either [asc] or [desc]");
        }

        return null;
    }

    public static IResult? ValidateSummaryInput(int year, int month)
    {
        if (year <= 0)
        {
            return Results.BadRequest("Year must be a positive integer.");
        }

        if (month < 1 || month > 12)
        {
            return Results.BadRequest("Month must be a valid integer from 1 to 12.");
        }

        return null;
    }
}
