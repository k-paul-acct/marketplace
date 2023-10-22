using System.Text.Json.Serialization;

namespace Marketplace.Api.Dto;

public class OrderDto
{
    [JsonPropertyName("orderId")]
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public int UserId { get; set; }
    public int? ProductId { get; set; }

    public ProductDto? Product { get; set; }
    public UserDto User { get; set; } = null!;
}