namespace Marketplace.Api.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int SellerId { get; set; }

    public virtual Category Category { get; set; } = null!;
    public virtual User Seller { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<UserHasProductInWishlist> WishedByUsers { get; set; } = new List<UserHasProductInWishlist>();
}