using Marketplace.Api.Data.Models;
using Marketplace.Api.Dto;

namespace Marketplace.Api.Mapping;

public static class OrderMapping
{
    public static OrderDto MapToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            ProductId = order.ProductId,
            UserId = order.UserId!.Value,
            TotalAmount = order.TotalAmount!.Value,
            TotalQuantity = order.TotalQuantity!.Value,
            OrderDate = order.CreateTime!.Value,
        };
    }
}