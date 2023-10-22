using Marketplace.Api.Data.Models;
using Marketplace.Api.Dto;

namespace Marketplace.Api.Mapping;

public static class UserMapping
{
    public static UserDto MapToDto(this User user)
    {
        var role = user.Roles.First();
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            ImageUrl = user.ImageUrl,
            PasswordHash = user.PasswordHash,
            RoleId = role.Id,
            Role = role.MapToDto(),
        };
    }
}