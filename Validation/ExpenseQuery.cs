public static class ExpenseQuery
{
    public static IQueryable<Expense> ApplyFilters(
        IQueryable<Expense> query,
        string? category,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        if (category != null)
        {
            query = query.Where(expense => expense.Category == category);
        }

        if (startDate != null)
        {
            query = query.Where(expense => expense.Date >= startDate);
        }

        if (endDate != null)
        {
            query = query.Where(expense => expense.Date <= endDate);
        }

        return query;
    }

    public static IQueryable<Expense> ApplySorting(
        IQueryable<Expense> query,
        string? sortBy,
        string? sortOrder)
    {
        if (sortBy == "date")
        {
            query = sortOrder == "desc"
                ? query.OrderByDescending(expense => expense.Date)
                : query.OrderBy(expense => expense.Date);
        }
        else if (sortBy == "amount")
        {
            query = sortOrder == "desc"
                ? query.OrderByDescending(expense => expense.Amount)
                : query.OrderBy(expense => expense.Amount);
        }

        return query;
    }
}
