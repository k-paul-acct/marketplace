using System.Text.Json.Serialization;
using Marketplace.Api.Types;

namespace Marketplace.Api.Models;

public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public Roles RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [JsonIgnore]
    public virtual ICollection<UserHasProductInWishlist> WishlistProducts { get; set; } = new List<UserHasProductInWishlist>();
}