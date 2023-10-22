namespace Marketplace.Api.Data.Models;

public class Category : IDbEntity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public int Id { get; set; }
}