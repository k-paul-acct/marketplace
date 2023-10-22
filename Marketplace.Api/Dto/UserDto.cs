using System.Text.Json.Serialization;

namespace Marketplace.Api.Dto;

public class UserDto
{
    [JsonPropertyName("userId")]
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public int RoleId { get; set; }

    public RoleDto Role { get; set; } = null!;
}