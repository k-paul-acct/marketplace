using API_Marketplace_.net_7_v1.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_Marketplace_.net_7_v1.Controllers
{
    public class OrderItemAPIHandler
    {
        public static async Task CreateOrderItemAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                var newOrderItem = JsonSerializer.Deserialize<OrderItem>(jsonBody);
                // Добавляем новую запись OrderItem в DbSet<OrderItem>
                dbContext.OrderItems.Add(newOrderItem);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = 201; // Created
                await context.Response.WriteAsync("OrderItem created successfully.");
            }
            catch (JsonException)
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid JSON data.");
            }
        }

        public static async Task GetOrderItemAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["OrderItemId"] is string orderItemIdStr && int.TryParse(orderItemIdStr, out int orderItemId))
            {
                var orderItem = await dbContext.OrderItems.FindAsync(orderItemId);

                if (orderItem != null)
                {
                    var orderItemJson = JsonSerializer.Serialize(orderItem);
                    context.Response.StatusCode = 200; // OK
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(orderItemJson);
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("OrderItem not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid OrderItem ID format.");
            }
        }

        public static async Task<string> GetAllOrderItemsAsync()
        {
            using var dbContext = new MarketplaceDbContext();
            var orderItems = dbContext.OrderItems.ToList();
            var orderItemsJsonArray = JsonSerializer.Serialize(orderItems);
            return orderItemsJsonArray;
        }

        public static async Task UpdateOrderItemAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["OrderItemId"] is string orderItemIdStr && int.TryParse(orderItemIdStr, out int orderItemId))
            {
                using var reader = new StreamReader(context.Request.Body);
                var jsonBody = await reader.ReadToEndAsync();

                try
                {
                    var orderItemToUpdate = await dbContext.OrderItems.FindAsync(orderItemId);
                    if (orderItemToUpdate != null)
                    {
                        var updatedOrderItem = JsonSerializer.Deserialize<OrderItem>(jsonBody);
                        orderItemToUpdate.OrderId = updatedOrderItem.OrderId;
                        orderItemToUpdate.ProductId = updatedOrderItem.ProductId;
                        orderItemToUpdate.Quantity = updatedOrderItem.Quantity;
                        orderItemToUpdate.Price = updatedOrderItem.Price;
                        // Сохраняем изменения в базе данных
                        await dbContext.SaveChangesAsync();
                        context.Response.StatusCode = 200; // OK
                        await context.Response.WriteAsync("OrderItem updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404; // Not Found
                        await context.Response.WriteAsync("OrderItem not found.");
                    }
                }
                catch (JsonException)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("Invalid JSON data.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid OrderItem ID format.");
            }
        }

        public static async Task DeleteOrderItemByIdAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["OrderItemId"] is string orderItemIdStr && int.TryParse(orderItemIdStr, out int orderItemId))
            {
                var orderItemToDelete = await dbContext.OrderItems.FindAsync(orderItemId);

                if (orderItemToDelete != null)
                {
                    dbContext.OrderItems.Remove(orderItemToDelete);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("OrderItem deleted successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("OrderItem not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid OrderItem ID format.");
            }
        }
    }
}
