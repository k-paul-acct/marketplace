namespace Marketplace.Api.Dto;

public class UserHasProductInWishlistDto
{
    public (int UserId, int ProductId) Id { get; set; }

    public UserDto User { get; set; } = null!;
    public ProductDto Product { get; set; } = null!;
}