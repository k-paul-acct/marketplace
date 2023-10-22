namespace Marketplace.Api.Data.Models;

public class Review : IDbEntity<int>
{
    public int? UserId { get; set; }
    public int? ProductId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? ImageUrl { get; set; }

    public virtual Product? Product { get; set; }
    public virtual User? User { get; set; }
    public int Id { get; set; }
}