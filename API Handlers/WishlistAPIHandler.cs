using API_Marketplace_.net_7_v1.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_Marketplace_.net_7_v1.Controllers
{
    public class WishlistAPIHandler
    {
        public static async Task CreateWishlistAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                // Десериализуем JSON в объект Wishlist
                var newWishlist = JsonSerializer.Deserialize<Wishlist>(jsonBody);

                // Добавляем список желаний в DbSet<Wishlist> и сохраняем изменения в базе данных
                dbContext.Wishlists.Add(newWishlist);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = 201; // Created
                await context.Response.WriteAsync("Wishlist created successfully.");
            }
            catch (JsonException)
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid JSON data.");
            }
        }

        public static async Task GetWishlistAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["WishlistId"] is string wishlistIdStr && int.TryParse(wishlistIdStr, out int wishlistId))
            {
                // Найдите список желаний в базе данных по WishlistId
                var wishlist = await dbContext.Wishlists.FindAsync(wishlistId);

                if (wishlist != null)
                {
                    // Сериализуйте список желаний в JSON и верните его клиенту
                    var wishlistJson = JsonSerializer.Serialize(wishlist);
                    context.Response.StatusCode = 200; // OK
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(wishlistJson);
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Wishlist not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Wishlist ID format.");
            }
        }

        public static async Task<string> GetAllWishlistsAsync()
        {
            using var dbContext = new MarketplaceDbContext();
            {
                var wishlists = dbContext.Wishlists.ToList();

                // Преобразуем список списков желаний в JSON
                var wishlistJsonArray = JsonSerializer.Serialize(wishlists);
                await Console.Out.WriteLineAsync(wishlistJsonArray);
                return wishlistJsonArray;
            }
        }

        public static async Task UpdateWishlistAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Получите WishlistId из URL
            if (context.Request.RouteValues["WishlistId"] is string wishlistIdStr && int.TryParse(wishlistIdStr, out int wishlistId))
            {
                // Читаем JSON-тело запроса
                using var reader = new StreamReader(context.Request.Body);
                var jsonBody = await reader.ReadToEndAsync();

                try
                {
                    // Десериализуем JSON в объект Wishlist и обновляем данные списка желаний
                    var wishlistToUpdate = await dbContext.Wishlists.FindAsync(wishlistId);
                    if (wishlistToUpdate != null)
                    {
                        var updatedWishlist = JsonSerializer.Deserialize<Wishlist>(jsonBody);
                        wishlistToUpdate.UserId = updatedWishlist.UserId;
                        wishlistToUpdate.ProductId = updatedWishlist.ProductId;
                        wishlistToUpdate.Product = updatedWishlist.Product;
                        wishlistToUpdate.User = updatedWishlist.User;

                        // Сохраняем изменения в базе данных
                        await dbContext.SaveChangesAsync();
                        context.Response.StatusCode = 200; // OK
                        await context.Response.WriteAsync("Wishlist updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404; // Not Found
                        await context.Response.WriteAsync("Wishlist not found.");
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
                await context.Response.WriteAsync("Invalid Wishlist ID format.");
            }
        }

        public static async Task DeleteWishlistByIDAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["WishlistId"] is string wishlistIdStr && int.TryParse(wishlistIdStr, out int wishlistId))
            {
                // Найдите список желаний в базе данных по WishlistId
                var wishlistToDelete = await dbContext.Wishlists.FindAsync(wishlistId);

                if (wishlistToDelete != null)
                {
                    // Удаляем список желаний из DbSet<Wishlist> и сохраняем изменения
                    dbContext.Wishlists.Remove(wishlistToDelete);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("Wishlist deleted successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Wishlist not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Wishlist ID format.");
            }
        }
    }
}
