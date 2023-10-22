namespace Marketplace.Api.Data.Models;

public class Order : IDbEntity<int>
{
    public int? UserId { get; set; }
    public int? ProductId { get; set; }
    public DateTime? CreateTime { get; set; }
    public int? TotalQuantity { get; set; }
    public decimal? TotalAmount { get; set; }

    public virtual Product? Product { get; set; }
    public virtual User? User { get; set; }
    public int Id { get; set; }
}