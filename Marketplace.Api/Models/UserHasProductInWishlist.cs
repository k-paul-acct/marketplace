namespace Marketplace.Api.Models;

public class UserHasProductInWishlist
{
    public int UserId { get; set; }
    public int ProductId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}