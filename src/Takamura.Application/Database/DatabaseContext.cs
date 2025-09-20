using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database.Entities.Bill;
using Takamura.Application.Database.Entities.Budget;
using Takamura.Application.Database.Entities.BudgetAllocation;
using Takamura.Application.Database.Entities.Category;
using Takamura.Application.Database.Entities.SubCategory;

namespace Takamura.Application.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) 
        :base(options)
    {
        
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HaveColumnType("decimal(18,2)");
        
        configurationBuilder.Properties<string>()
            .HaveColumnType("varchar(100)");
        
        configurationBuilder.Properties<Enum>()
            .HaveConversion<string>();
        
        base.ConfigureConventions(configurationBuilder);
    }

    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    
    public DbSet<SubCategoryEntity> SubCategories => Set<SubCategoryEntity>();
    
    public DbSet<BudgetEntity> Budgets => Set<BudgetEntity>();
    
    public DbSet<BudgetAllocationEntity> BudgetAllocations => Set<BudgetAllocationEntity>();
    
    public DbSet<BillEntity> Bills => Set<BillEntity>();
}