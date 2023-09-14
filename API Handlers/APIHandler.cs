using API_Marketplace_.net_7_v1.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_Marketplace_.net_7_v1.API_Handlers
{
    public class APIHandler
    {
        public static async Task CreateEntityAsync<T>(HttpContext context, MarketplaceDbContext dbContext) where T : class
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                // Десериализуем JSON в объект типа T
                var newEntity = JsonSerializer.Deserialize<T>(jsonBody);

                // Добавляем объект в DbSet<T> и сохраняем изменения в базе данных
                dbContext.Set<T>().Add(newEntity);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = 201; // Created
                await context.Response.WriteAsync($"{typeof(T).Name} created successfully.");
            }
            catch (JsonException)
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid JSON data.");
            }
        }

        public static async Task GetEntityAsync<T>(HttpContext context, MarketplaceDbContext dbContext) where T : class
        {
            if (context.Request.RouteValues["EntityId"] is string entityIdStr && int.TryParse(entityIdStr, out int entityId))
            {
                // Найдите объект типа T в базе данных по entityId
                var entity = await dbContext.Set<T>().FindAsync(entityId);

                if (entity != null)
                {
                    // Сериализуйте объект типа T в JSON и верните его клиенту
                    var entityJson = JsonSerializer.Serialize(entity);
                    context.Response.StatusCode = 200; // OK
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(entityJson);
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync($"{typeof(T).Name} not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync($"Invalid {typeof(T).Name} ID format.");
            }
        }

        public static async Task GetAllEntitiesAsync<T>(MarketplaceDbContext dbContext, HttpContext context) where T : class
        {
            var entityJson = JsonSerializer.Serialize(await dbContext.Set<T>().ToListAsync());
            await context.Response.WriteAsJsonAsync(entityJson);
        }

        public static async Task UpdateEntityAsync<T>(HttpContext context, MarketplaceDbContext dbContext) where T : class
        {
            // Получите EntityId из URL
            if (context.Request.RouteValues["Id"] is string entityIdStr && int.TryParse(entityIdStr, out int entityId))
            {
                // Читаем JSON-тело запроса
                using var reader = new StreamReader(context.Request.Body);
                var jsonBody = await reader.ReadToEndAsync();

                try
                {
                    // Десериализуем JSON в объект типа T и обновляем данные объекта
                    var entityToUpdate = await dbContext.Set<T>().FindAsync(entityId);
                    if (entityToUpdate != null)
                    {
                        var updatedEntity = JsonSerializer.Deserialize<T>(jsonBody);

                        // Модифицируем объект типа T, заменяя только не null значения полей
                        foreach (var propertyInfo in typeof(T).GetProperties())
                        {
                            // Исключаем поле с именем "Id" (или другое поле, которое не должно изменяться)
                            if (!(propertyInfo.Name.Contains("ID") || propertyInfo.Name.Contains("Id")))
                            {
                                var newValue = propertyInfo.GetValue(updatedEntity);
                                if (newValue != null)
                                {
                                    propertyInfo.SetValue(entityToUpdate, newValue);
                                }
                            }
                        }


                        // Сохраняем изменения в базе данных
                        await dbContext.SaveChangesAsync();
                        context.Response.StatusCode = 200; // OK
                        await context.Response.WriteAsync($"{typeof(T).Name} updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404; // Not Found
                        await context.Response.WriteAsync($"{typeof(T).Name} not found.");
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
                await context.Response.WriteAsync($"Invalid {typeof(T).Name} ID format.");
            }
        }


        public static async Task DeleteEntityByIDAsync<T>(HttpContext context, MarketplaceDbContext dbContext) where T : class
        {
            if (context.Request.RouteValues["EntityId"] is string entityIdStr && int.TryParse(entityIdStr, out int entityId))
            {
                // Найдите объект типа T в базе данных по entityId
                var entityToDelete = await dbContext.Set<T>().FindAsync(entityId);

                if (entityToDelete != null)
                {
                    // Удаляем объект из DbSet<T> и сохраняем изменения
                    dbContext.Set<T>().Remove(entityToDelete);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync($"{typeof(T).Name} deleted successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync($"{typeof(T).Name} not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync($"Invalid {typeof(T).Name} ID format.");
            }
        }

    }
}
