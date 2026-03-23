using Microsoft.EntityFrameworkCore;

public class ExpenseContext(DbContextOptions<ExpenseContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; }
}