var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/expenses", () =>
{
    List<Expense> expenseList = [];

    expenseList.Add(new Expense
    { 
        Id = 1,
        Amount = 50,
        Category = "Indkøb",
        Description = "Aftensmad i Netto",
        Date = new DateOnly(2026, 3, 22)
    });
    expenseList.Add(new Expense
    { 
        Id = 2,
        Amount = 100,
        Category = "Øl",
        Description = "Sjov",
        Date = new DateOnly(2026, 3, 22)
    });
    expenseList.Add(new Expense
    { 
        Id = 3,
        Amount = 10000,
        Category = "Husleje",
        Description = "Månedlig Husleje",
        Date = new DateOnly(2026, 3, 22)
    });

    return expenseList;
})
.WithName("GetExpenses");

app.Run();
