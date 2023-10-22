namespace Marketplace.Api.Data.Models;

public class Role : IDbEntity<int>
{
    public string? RoleName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public int Id { get; set; }
}