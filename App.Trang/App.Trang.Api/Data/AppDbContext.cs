using App.Trang.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Data;

/// <summary>
/// Database context cho ứng dụng Web Bán Hàng Trang
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<WareHouse> WareHouses => Set<WareHouse>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== Provider =====
        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // ===== Customer =====
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // ===== Category (self-reference) =====
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Parent)
                  .WithMany(e => e.Children)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);
        });

        // ===== Product =====
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== WareHouse (1-N with Product) =====
        modelBuilder.Entity<WareHouse>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.ProviderId }).IsUnique();

            entity.HasOne(e => e.Product)
                  .WithMany(e => e.WareHouses)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Provider)
                  .WithMany(e => e.WareHouses)
                  .HasForeignKey(e => e.ProviderId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);
        });

        // ===== Order =====
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Provider)
                  .WithMany(e => e.Orders)
                  .HasForeignKey(e => e.ProviderId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);

            entity.HasOne(e => e.Customer)
                  .WithMany(e => e.Orders)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);
        });

        // ===== OrderDetail =====
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasOne(e => e.Order)
                  .WithMany(e => e.OrderDetails)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany(e => e.OrderDetails)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// Tự động cập nhật UpdatedAt khi SaveChanges
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Tự động cập nhật UpdatedAt khi SaveChangesAsync
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
