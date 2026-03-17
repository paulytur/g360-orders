using Microsoft.EntityFrameworkCore;
using G360.Orders.Domain.Entities;

namespace G360.Orders.Infrastructure.Data;

/// <summary>
/// Entity Framework database context for the G360 Orders application (SQLite).
/// </summary>
public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public DbSet<PizzaType> PizzaTypes => Set<PizzaType>();
    public DbSet<PizzaDetail> PizzaDetails => Set<PizzaDetail>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.CreatedDatetime);
            e.HasMany(x => x.OrderDetails).WithOne(x => x.Order).HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderDetail>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.OrderId);
            e.HasIndex(x => x.PizzaId);
            e.HasOne(x => x.Order).WithMany(x => x.OrderDetails).HasForeignKey(x => x.OrderId);
            e.HasOne(x => x.Pizza).WithMany().HasForeignKey(x => x.PizzaId);
        });

        modelBuilder.Entity<Pizza>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Code);
            e.HasIndex(x => x.PizzaTypeId);
            e.HasOne(x => x.PizzaType).WithMany(x => x.Pizzas).HasForeignKey(x => x.PizzaTypeId).IsRequired(false);
        });

        modelBuilder.Entity<PizzaType>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Code);
            e.HasIndex(x => x.CategoryId);
            e.HasMany(x => x.PizzaDetails).WithOne(x => x.PizzaType).HasForeignKey(x => x.PizzaTypeId);
            e.HasOne(x => x.Category).WithMany(x => x.PizzaTypes).HasForeignKey(x => x.CategoryId).IsRequired(false);
        });

        modelBuilder.Entity<PizzaDetail>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.PizzaTypeId);
            e.HasIndex(x => x.IngredientId);
            e.HasIndex(x => new { x.PizzaTypeId, x.IngredientId }).IsUnique();
            e.HasOne(x => x.Ingredient).WithMany().HasForeignKey(x => x.IngredientId);
            e.HasOne(x => x.PizzaType).WithMany(x => x.PizzaDetails).HasForeignKey(x => x.PizzaTypeId);
        });

        modelBuilder.Entity<Ingredient>(e =>
        {
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name);
        });
    }
}
