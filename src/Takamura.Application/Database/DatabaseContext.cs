using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database.Entities;

namespace Takamura.Application.Database;

public class DatabaseContext : DbContext
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<SubCategory> SubCategories { get; set; } = null!;
    public DbSet<Budget> Budgets { get; set; } = null!;
    
    public DbSet<BudgetAllocation> BudgetAllocations { get; set; } = null!;
    
    public DbSet<Bill> Bills { get; set; } = null!;
}