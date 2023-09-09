using API_Marketplace_.net_7_v1.Models;
using System.Text.Json;

namespace API_Marketplace_.net_7_v1.Controllers
{
    public class ProductAPIHandler
    {
        public static async Task CreateProductAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                // Десериализуем JSON в объект Product
                var newProduct = JsonSerializer.Deserialize<Product>(jsonBody);
                newProduct.CreatedAt = DateTime.Now;
                // Добавляем продукт в DbSet<Product> и сохраняем изменения в базе данных
                dbContext.Products.Add(newProduct);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = 201; // Created
                await context.Response.WriteAsync("Product created successfully.");
            }
            catch (JsonException)
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid JSON data.");
            }
        }

        public static async Task GetProductAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["productId"] is string productIdStr && int.TryParse(productIdStr, out int productId))
            {
                // Найдите продукт в базе данных по ProductId
                var product = await dbContext.Products.FindAsync(productId);

                if (product != null)
                {
                    // Сериализуйте продукт в JSON и верните его клиенту
                    var productJson = JsonSerializer.Serialize(product);
                    context.Response.StatusCode = 200; // OK
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(productJson);
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Product not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid product ID format.");
            }
        }
        public static async Task<string> GetAllProductsAsync()
        {
            using var dbContext = new MarketplaceDbContext();
            {
                var users = dbContext.Products.ToList();

                // Преобразуем список пользователей в JSON
                var userJsonArray = JsonSerializer.Serialize(users);
                await Console.Out.WriteLineAsync(userJsonArray);
                return userJsonArray;
            }
        }

        public static async Task UpdateProductAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Получите ProductId из URL
            if (context.Request.RouteValues["productId"] is string productIdStr && int.TryParse(productIdStr, out int productId))
            {
                // Читаем JSON-тело запроса
                using var reader = new StreamReader(context.Request.Body);
                var jsonBody = await reader.ReadToEndAsync();

                try
                {
                    // Десериализуем JSON в объект Product и обновляем данные продукта
                    var productToUpdate = await dbContext.Products.FindAsync(productId);
                    if (productToUpdate != null)
                    {
                        var updatedProduct = JsonSerializer.Deserialize<Product>(jsonBody);
                        productToUpdate.Name = updatedProduct.Name;
                        productToUpdate.Description = updatedProduct.Description;
                        productToUpdate.Price = updatedProduct.Price;
                        productToUpdate.StockQuantity = updatedProduct.StockQuantity;
                        productToUpdate.CategoryId = updatedProduct.CategoryId;
                        productToUpdate.SellerUserId = updatedProduct.SellerUserId;
                        productToUpdate.UpdatedAt = DateTime.Now;
                        // Сохраняем изменения в базе данных
                        await dbContext.SaveChangesAsync();
                        context.Response.StatusCode = 200; // OK
                        await context.Response.WriteAsync("Product updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404; // Not Found
                        await context.Response.WriteAsync("Product not found.");
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
                await context.Response.WriteAsync("Invalid product ID format.");
            }
        }

        public static async Task DeleteProductAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["productId"] is string productIdStr && int.TryParse(productIdStr, out int productId))
            {
                // Найдите продукт в базе данных по ProductId
                var productToDelete = await dbContext.Products.FindAsync(productId);

                if (productToDelete != null)
                {
                    // Удаляем продукт из DbSet<Product> и сохраняем изменения
                    dbContext.Products.Remove(productToDelete);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("Product deleted successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Product not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid product ID format.");
            }
        }

    }
}
