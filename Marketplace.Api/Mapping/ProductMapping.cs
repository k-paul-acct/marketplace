using Marketplace.Api.Data.Models;
using Marketplace.Api.Dto;

namespace Marketplace.Api.Mapping;

public static class ProductMapping
{
    public static ProductDto MapToDto(this Product product)
    {
        var category = product.Categories.First();
        var seller = product.Users.First();
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name!,
            Description = product.Description,
            Price = product.Price!.Value,
            CreatedAt = product.CreatedAt!.Value,
            CategoryId = category.Id,
            Category = category.MapToDto(),
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity!.Value,
            UpdatedAt = product.UpdatedAt,
            SellerId = seller.Id,
            Seller = seller.MapToDto(),
        };
    }
}