using System.Text.Json.Serialization;

namespace Marketplace.Api.Dto;

public class RoleDto
{
    [JsonPropertyName("roleId")]
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;
}