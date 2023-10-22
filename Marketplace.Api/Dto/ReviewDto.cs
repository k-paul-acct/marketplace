using System.Text.Json.Serialization;

namespace Marketplace.Api.Dto;

public class ReviewDto
{
    [JsonPropertyName("reviewId")]
    public int Id { get; set; }

    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }

    public ProductDto Product { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}