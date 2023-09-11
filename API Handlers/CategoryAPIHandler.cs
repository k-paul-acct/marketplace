using API_Marketplace_.net_7_v1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_Marketplace_.net_7_v1.Controllers
{
    public class CategoryAPIHandler
    {
        public static async Task CreateCategoryAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                // Десериализуем JSON в объект Category
                var newCategory = JsonSerializer.Deserialize<Category>(jsonBody);

                // Добавляем категорию в DbSet<Category> и сохраняем изменения в базе данных
                dbContext.Categories.Add(newCategory);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = 201; // Created
                await context.Response.WriteAsync("Category created successfully.");
            }
            catch (JsonException)
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid JSON data.");
            }
        }

        public static async Task GetCategoryAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["CategoryId"] is string categoryIdStr && int.TryParse(categoryIdStr, out int categoryId))
            {
                // Найдите категорию в базе данных по CategoryId
                var category = await dbContext.Categories.FindAsync(categoryId);

                if (category != null)
                {
                    // Сериализуйте категорию в JSON и верните ее клиенту
                    var categoryJson = JsonSerializer.Serialize(category);
                    context.Response.StatusCode = 200; // OK
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(categoryJson);
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Category not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Category ID format.");
            }
        }

        public static async Task<string> GetAllCategoriesAsync()
        {
            using var dbContext = new MarketplaceDbContext();
            {
                var categories = await dbContext.Categories.ToListAsync();

                // Преобразуем список категорий в JSON
                var categoryJsonArray = JsonSerializer.Serialize(categories);
                await Console.Out.WriteLineAsync(categoryJsonArray);
                return categoryJsonArray;
            }
        }

        public static async Task UpdateCategoryAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Получите CategoryId из URL
            if (context.Request.RouteValues["CategoryId"] is string categoryIdStr && int.TryParse(categoryIdStr, out int categoryId))
            {
                // Читаем JSON-тело запроса
                using var reader = new StreamReader(context.Request.Body);
                var jsonBody = await reader.ReadToEndAsync();

                try
                {
                    // Десериализуем JSON в объект Category и обновляем данные категории
                    var categoryToUpdate = await dbContext.Categories.FindAsync(categoryId);
                    if (categoryToUpdate != null)
                    {
                        var updatedCategory = JsonSerializer.Deserialize<Category>(jsonBody);
                        categoryToUpdate.Name = updatedCategory.Name;
                        categoryToUpdate.Description = updatedCategory.Description;

                        // Сохраняем изменения в базе данных
                        await dbContext.SaveChangesAsync();
                        context.Response.StatusCode = 200; // OK
                        await context.Response.WriteAsync("Category updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404; // Not Found
                        await context.Response.WriteAsync("Category not found.");
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
                await context.Response.WriteAsync("Invalid Category ID format.");
            }
        }

        public static async Task DeleteCategoryByIDAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["CategoryId"] is string categoryIdStr && int.TryParse(categoryIdStr, out int categoryId))
            {
                // Найдите категорию в базе данных по CategoryId
                var categoryToDelete = await dbContext.Categories.FindAsync(categoryId);

                if (categoryToDelete != null)
                {
                    // Удаляем категорию из DbSet<Category> и сохраняем изменения
                    dbContext.Categories.Remove(categoryToDelete);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("Category deleted successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Category not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid Category ID format.");
            }
        }
    }
}
