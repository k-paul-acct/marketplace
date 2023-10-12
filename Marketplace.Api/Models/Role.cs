using System.Text.Json.Serialization;
using Marketplace.Api.Types;

namespace Marketplace.Api.Models;

public class Role
{
    public Roles RoleId { get; set; }
    public string RoleName { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}