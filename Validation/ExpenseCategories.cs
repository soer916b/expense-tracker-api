using System.Collections.Immutable;

public static class ExpenseCategories
{
    public static readonly ImmutableArray<string> Permitted =
    [
        "Groceries",
        "Food",
        "Transport",
        "Rent",
        "Bills",
        "Other"
    ];
}
