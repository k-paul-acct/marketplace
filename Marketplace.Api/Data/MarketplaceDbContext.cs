#nullable disable

using Marketplace.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Data;

public sealed class MarketplaceDbContext : DbContext
{
    public MarketplaceDbContext()
    {
    }

    public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__19093A2B6A5074A5");

            entity.Property(e => e.Id).HasColumnName("CategoryID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__C3905BAF9441CB31");

            entity.Property(e => e.Id).HasColumnName("OrderID");
            entity.Property(e => e.CreateTime).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Orders__ProductI__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Orders__UserID__5070F446");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__B40CC6ED6FF2FA8D");

            entity.Property(e => e.Id).HasColumnName("ProductID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasMany(d => d.Categories).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductsCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK__ProductsC__Categ__3C69FB99"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK__ProductsC__Produ__3B75D760"),
                    j =>
                    {
                        j.HasKey("ProductId", "CategoryId").HasName("PK__Products__159C554F79BAE9A6");
                        j.ToTable("ProductsCategories");
                        j.IndexerProperty<int>("ProductId").HasColumnName("ProductID");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("CategoryID");
                    });
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__74BC79AEE0D9F1FA");

            entity.Property(e => e.Id).HasColumnName("ReviewID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Reviews__Product__5535A963");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Reviews__UserID__5441852A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__8AFACE3A49FB838A");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616078B37FFC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__1788CCAC44D74AC0");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534AE6E4288").IsUnique();

            entity.Property(e => e.Id).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(11);

            entity.HasMany(d => d.Products).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserProduct",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK__UserProdu__Produ__49C3F6B7"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserProdu__UserI__48CFD27E"),
                    j =>
                    {
                        j.HasKey("UserId", "ProductId").HasName("PK__UserProd__DCC800C2BE1C88EA");
                        j.ToTable("UserProducts");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("ProductId").HasColumnName("ProductID");
                    });

            entity.HasMany(d => d.ProductsNavigation).WithMany(p => p.UsersNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "Wishlist",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK__Wishlists__Produ__4D94879B"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Wishlists__UserI__4CA06362"),
                    j =>
                    {
                        j.HasKey("UserId", "ProductId").HasName("PK__Wishlist__DCC800C2F5BEF5E8");
                        j.ToTable("Wishlists");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("ProductId").HasColumnName("ProductID");
                    });

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__UserRoles__RoleI__45F365D3"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserRoles__UserI__44FF419A"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF27604FFB9A40C4");
                        j.ToTable("UserRoles");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });
    }
}