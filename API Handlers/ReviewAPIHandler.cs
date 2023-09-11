using API_Marketplace_.net_7_v1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_Marketplace_.net_7_v1.Controllers
{
    public class ReviewAPIHandler
    {
        public static async Task CreateReviewAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                // Десериализуем JSON в объект Review
                var newReview = JsonSerializer.Deserialize<Review>(jsonBody);
                newReview.CreatedAt = DateTime.Now;
                // Добавляем отзыв в DbSet<Review> и сохраняем изменения в базе данных
                dbContext.Reviews.Add(newReview);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = 201; // Created
                await context.Response.WriteAsync("Review created successfully.");
            }
            catch (JsonException)
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid JSON data.");
            }
        }

        public static async Task GetReviewAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["reviewId"] is string reviewIdStr && int.TryParse(reviewIdStr, out int reviewId))
            {
                // Найдите отзыв в базе данных по ReviewId
                var review = await dbContext.Reviews.FindAsync(reviewId);

                if (review != null)
                {
                    // Сериализуйте отзыв в JSON и верните его клиенту
                    var reviewJson = JsonSerializer.Serialize(review);
                    context.Response.StatusCode = 200; // OK
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(reviewJson);
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Review not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Review ID format.");
            }
        }

        public static async Task<string> GetAllReviewsAsync()
        {
            using var dbContext = new MarketplaceDbContext();
            {
                var reviews = dbContext.Reviews.ToList();

                // Преобразуем список отзывов в JSON
                var reviewJsonArray = JsonSerializer.Serialize(reviews);
                await Console.Out.WriteLineAsync(reviewJsonArray);
                return reviewJsonArray;
            }
        }

        public static async Task UpdateReviewAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Получите ReviewId из URL
            if (context.Request.RouteValues["reviewId"] is string reviewIdStr && int.TryParse(reviewIdStr, out int reviewId))
            {
                // Читаем JSON-тело запроса
                using var reader = new StreamReader(context.Request.Body);
                var jsonBody = await reader.ReadToEndAsync();

                try
                {
                    // Десериализуем JSON в объект Review и обновляем данные отзыва
                    var reviewToUpdate = await dbContext.Reviews.FindAsync(reviewId);
                    if (reviewToUpdate != null)
                    {
                        var updatedReview = JsonSerializer.Deserialize<Review>(jsonBody);
                        reviewToUpdate.UserId = updatedReview.UserId;
                        reviewToUpdate.ProductId = updatedReview.ProductId;
                        reviewToUpdate.Rating = updatedReview.Rating;
                        reviewToUpdate.Comment = updatedReview.Comment;
                        reviewToUpdate.CreatedAt = DateTime.Now;
                        reviewToUpdate.ImageURL = updatedReview.ImageURL;
                        // Сохраняем изменения в базе данных
                        await dbContext.SaveChangesAsync();
                        context.Response.StatusCode = 200; // OK
                        await context.Response.WriteAsync("Review updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404; // Not Found
                        await context.Response.WriteAsync("Review not found.");
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
                await context.Response.WriteAsync("Invalid Review ID format.");
            }
        }

        public static async Task DeleteReviewByIdAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["reviewId"] is string reviewIdStr && int.TryParse(reviewIdStr, out int reviewId))
            {
                // Найдите отзыв в базе данных по ReviewId
                var reviewToDelete = await dbContext.Reviews.FindAsync(reviewId);

                if (reviewToDelete != null)
                {
                    // Удаляем отзыв из DbSet<Review> и сохраняем изменения
                    dbContext.Reviews.Remove(reviewToDelete);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("Review deleted successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Review not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Review ID format.");
            }
        }
    }
}
