using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

public class ExpenseEndpointsTests : IClassFixture<ExpenseTrackerApplicationFactory>
{
    private readonly ExpenseTrackerApplicationFactory _factory;
    private readonly HttpClient _client;

    public ExpenseEndpointsTests(ExpenseTrackerApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostExpense_CreatesExpense()
    {
        await _factory.ResetDatabaseAsync();

        var request = new ExpenseRequest(50.00m, "Food", "Lunch", new DateOnly(2026, 3, 10));

        var response = await _client.PostAsJsonAsync("/expenses", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var createdExpense = await response.Content.ReadFromJsonAsync<ExpenseResponse>();
        createdExpense.ShouldNotBeNull();
        createdExpense.Id.ShouldBeGreaterThan(0);
        createdExpense.Amount.ShouldBe(request.Amount);
        createdExpense.Category.ShouldBe(request.Category);
        createdExpense.Description.ShouldBe(request.Description);
        createdExpense.Date.ShouldBe(request.Date);
    }

    [Fact]
    public async Task GetExpenses_FiltersAndSortsByAmountDescending()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedExpensesAsync(
            new Expense { Amount = 10.00m, Category = "Food", Description = "Coffee", Date = new DateOnly(2026, 3, 1) },
            new Expense { Amount = 50.00m, Category = "Food", Description = "Dinner", Date = new DateOnly(2026, 3, 2) },
            new Expense { Amount = 20.00m, Category = "Transport", Description = "Bus", Date = new DateOnly(2026, 3, 3) });

        var expenses = await _client.GetFromJsonAsync<List<ExpenseResponse>>(
            "/expenses?category=Food&sortBy=amount&sortOrder=desc");

        expenses.ShouldNotBeNull();
        expenses.Count.ShouldBe(2);
        expenses[0].Amount.ShouldBe(50.00m);
        expenses[1].Amount.ShouldBe(10.00m);
    }

    [Fact]
    public async Task GetExpenseById_ReturnsExpense()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedExpensesAsync(
            new Expense { Amount = 42.50m, Category = "Other", Description = "Book", Date = new DateOnly(2026, 3, 8) });

        var expenses = await _client.GetFromJsonAsync<List<ExpenseResponse>>("/expenses");

        expenses.ShouldNotBeNull();
        expenses.Count.ShouldBe(1);

        var expense = await _client.GetFromJsonAsync<ExpenseResponse>($"/expenses/{expenses[0].Id}");

        expense.ShouldNotBeNull();
        expense.Id.ShouldBe(expenses[0].Id);
        expense.Amount.ShouldBe(42.50m);
        expense.Category.ShouldBe("Other");
        expense.Description.ShouldBe("Book");
        expense.Date.ShouldBe(new DateOnly(2026, 3, 8));
    }

    [Fact]
    public async Task GetExpenseById_ForMissingExpense_ReturnsNotFound()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/expenses/999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostExpense_WithInvalidCategory_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();

        var request = new ExpenseRequest(25.00m, "Beer", "Unsupported", new DateOnly(2026, 3, 12));

        var response = await _client.PostAsJsonAsync("/expenses", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadAsStringAsync();
        body.ShouldContain("Groceries");
        body.ShouldContain("Other");
    }

    [Fact]
    public async Task GetSummary_ReturnsMonthlyCountAndTotal()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedExpensesAsync(
            new Expense { Amount = 50.00m, Category = "Food", Description = "Lunch", Date = new DateOnly(2026, 3, 10) },
            new Expense { Amount = 120.00m, Category = "Transport", Description = "Train", Date = new DateOnly(2026, 3, 15) },
            new Expense { Amount = 300.00m, Category = "Rent", Description = "April rent", Date = new DateOnly(2026, 4, 1) });

        var summary = await _client.GetFromJsonAsync<ExpenseSummaryResponse>(
            "/expenses/summary?year=2026&month=3");

        summary.ShouldNotBeNull();
        summary.Year.ShouldBe(2026);
        summary.Month.ShouldBe(3);
        summary.ExpenseCount.ShouldBe(2);
        summary.TotalAmount.ShouldBe(170.00m);
    }

    [Fact]
    public async Task DeleteExpense_RemovesExpense()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedExpensesAsync(
            new Expense { Amount = 15.00m, Category = "Food", Description = "Snack", Date = new DateOnly(2026, 3, 4) });

        var expenses = await _client.GetFromJsonAsync<List<ExpenseResponse>>("/expenses");

        expenses.ShouldNotBeNull();
        expenses.Count.ShouldBe(1);

        var deleteResponse = await _client.DeleteAsync($"/expenses/{expenses[0].Id}");

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deletedExpense = await deleteResponse.Content.ReadFromJsonAsync<ExpenseResponse>();
        deletedExpense.ShouldNotBeNull();
        deletedExpense.Id.ShouldBe(expenses[0].Id);

        var remainingExpenses = await _client.GetFromJsonAsync<List<ExpenseResponse>>("/expenses");

        remainingExpenses.ShouldNotBeNull();
        remainingExpenses.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteExpense_ForMissingExpense_ReturnsNotFound()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.DeleteAsync("/expenses/999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var body = await response.Content.ReadAsStringAsync();
        body.ShouldContain("Expense not found.");
    }

    [Fact]
    public async Task PutExpense_ForMissingExpense_ReturnsNotFound()
    {
        await _factory.ResetDatabaseAsync();

        var request = new ExpenseRequest(100.00m, "Other", "Should not exist", new DateOnly(2026, 3, 23));

        var response = await _client.PutAsJsonAsync("/expenses/999", request);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("/expenses?category=")]
    [InlineData("/expenses?category=Beer")]
    [InlineData("/expenses?startDate=2026-03-31&endDate=2026-03-01")]
    [InlineData("/expenses?sortOrder=desc")]
    [InlineData("/expenses?sortBy=category")]
    [InlineData("/expenses?sortBy=amount&sortOrder=down")]
    public async Task GetExpenses_WithInvalidQuery_ReturnsBadRequest(string uri)
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync(uri);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    private sealed record ExpenseRequest(decimal Amount, string Category, string Description, DateOnly Date);

    private sealed class ExpenseResponse
    {
        public int Id { get; init; }
        public decimal Amount { get; init; }
        public string Category { get; init; } = "";
        public string Description { get; init; } = "";
        public DateOnly Date { get; init; }
    }

    private sealed class ExpenseSummaryResponse
    {
        public int Year { get; init; }
        public int Month { get; init; }
        public int ExpenseCount { get; init; }
        public decimal TotalAmount { get; init; }
    }
}
