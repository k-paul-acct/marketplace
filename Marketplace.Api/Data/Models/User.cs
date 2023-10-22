namespace Marketplace.Api.Data.Models;

public class User : IDbEntity<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Product> ProductsNavigation { get; set; } = new List<Product>();
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    public int Id { get; set; }
}