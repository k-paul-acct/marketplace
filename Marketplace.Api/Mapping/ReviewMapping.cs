using Marketplace.Api.Data.Models;
using Marketplace.Api.Dto;

namespace Marketplace.Api.Mapping;

public static class ReviewMapping
{
    public static ReviewDto MapToDto(this Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            ProductId = review.ProductId!.Value,
            UserId = review.UserId!.Value,
            Comment = review.Comment!,
            Rating = review.Rating!.Value,
            CreatedAt = review.CreatedAt!.Value,
            ImageUrl = review.ImageUrl,
        };
    }
}