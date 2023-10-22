using Marketplace.Api.Data.Models;
using Marketplace.Api.Dto;

namespace Marketplace.Api.Mapping;

public static class RoleMapping
{
    public static RoleDto MapToDto(this Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName!,
        };
    }
}