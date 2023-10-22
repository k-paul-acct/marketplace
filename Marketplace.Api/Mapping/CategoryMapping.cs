using Marketplace.Api.Data.Models;
using Marketplace.Api.Dto;

namespace Marketplace.Api.Mapping;

public static class CategoryMapping
{
    public static CategoryDto MapToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name!,
            Description = category.Description,
        };
    }
}