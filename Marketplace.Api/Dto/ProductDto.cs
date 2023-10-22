using System.Text.Json.Serialization;

namespace Marketplace.Api.Dto;

public class ProductDto
{
    [JsonPropertyName("productId")]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int SellerId { get; set; }

    public CategoryDto Category { get; set; } = null!;
    public UserDto Seller { get; set; } = null!;
}