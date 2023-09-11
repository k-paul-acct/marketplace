using API_Marketplace_.net_7_v1.Models;
using System.Text.Json;

namespace API_Marketplace_.net_7_v1.Controllers
{
    public class OrderAPIHandler
    {
        public static async Task CreateOrderAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                // Десериализуем JSON в объект Order
                var newOrder = JsonSerializer.Deserialize<Order>(jsonBody);
                newOrder.OrderDate = DateTime.Now;
                // Добавляем продукт в DbSet<Order> и сохраняем изменения в базе данных
                dbContext.Orders.Add(newOrder);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = 201; // Created
                await context.Response.WriteAsync("Order created successfully.");
            }
            catch (JsonException)
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid JSON data.");
            }
        }

        public static async Task GetOrderAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["OrderId"] is string OrderIdStr && int.TryParse(OrderIdStr, out int OrderId))
            {
                // Найдите продукт в базе данных по OrderId
                var Order = await dbContext.Orders.FindAsync(OrderId);

                if (Order != null)
                {
                    // Сериализуйте продукт в JSON и верните его клиенту
                    var OrderJson = JsonSerializer.Serialize(Order);
                    context.Response.StatusCode = 200; // OK
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(OrderJson);
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Order not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Order ID format.");
            }
        }
        public static async Task<string> GetAllOrdersAsync()
        {
            using var dbContext = new MarketplaceDbContext();
            {
                var users = dbContext.Orders.ToList();

                // Преобразуем список пользователей в JSON
                var userJsonArray = JsonSerializer.Serialize(users);
                await Console.Out.WriteLineAsync(userJsonArray);
                return userJsonArray;
            }
        }

        public static async Task UpdateOrderAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Получите OrderId из URL
            if (context.Request.RouteValues["OrderId"] is string OrderIdStr && int.TryParse(OrderIdStr, out int OrderId))
            {
                // Читаем JSON-тело запроса
                using var reader = new StreamReader(context.Request.Body);
                var jsonBody = await reader.ReadToEndAsync();

                try
                {
                    // Десериализуем JSON в объект Order и обновляем данные продукта
                    var OrderToUpdate = await dbContext.Orders.FindAsync(OrderId);
                    if (OrderToUpdate != null)
                    {
                        var updatedOrder = JsonSerializer.Deserialize<Order>(jsonBody);
                        OrderToUpdate.UserId = updatedOrder.UserId;
                        OrderToUpdate.OrderItems = updatedOrder.OrderItems;
                        OrderToUpdate.User = updatedOrder.User;
                        OrderToUpdate.TotalAmount = updatedOrder.TotalAmount;
                        OrderToUpdate.OrderDate = DateTime.Now;
                        // Сохраняем изменения в базе данных
                        await dbContext.SaveChangesAsync();
                        context.Response.StatusCode = 200; // OK
                        await context.Response.WriteAsync("Order updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404; // Not Found
                        await context.Response.WriteAsync("Order not found.");
                    }
                }
                catch (JsonException)
                {
                    // Ошибка в формате JSON-данных, возвращаем ошибку
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("Invalid JSON data.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Order ID format.");
            }
        }

        public static async Task DeleteOrderByIDAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["OrderId"] is string OrderIdStr && int.TryParse(OrderIdStr, out int OrderId))
            {
                // Найдите продукт в базе данных по OrderId
                var OrderToDelete = await dbContext.Orders.FindAsync(OrderId);

                if (OrderToDelete != null)
                {
                    // Удаляем продукт из DbSet<Order> и сохраняем изменения
                    dbContext.Orders.Remove(OrderToDelete);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("Order deleted successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Order not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Order ID format.");
            }
        }

    }
}
