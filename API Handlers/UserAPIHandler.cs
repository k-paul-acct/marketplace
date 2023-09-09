using API_Marketplace_.net_7_v1.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace API_Marketplace_.net_7_v1.Controllers
{
    public class UserAPIHandler
    {
        public static async Task CreateUserAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                // Десериализуем JSON в объект User
                var newUser = JsonSerializer.Deserialize<User>(jsonBody);
                // Добавляем пользователя в DbSet<User> и сохраняем изменения в базе данных
                dbContext.Users.Add(newUser);
                await dbContext.SaveChangesAsync();

                // Возвращаем успешный ответ
                context.Response.StatusCode = 201; // Created
                await context.Response.WriteAsync("User created successfully.");
            }
            catch (JsonException)
            {
                // Ошибка в формате JSON-данных, возвращаем ошибку
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid JSON data.");
            }
        }

        public static async Task DeleteUserAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["userId"] is string userIdStr && int.TryParse(userIdStr, out int userId))
            {
                // Найдите пользователя в базе данных
                var userToDelete = await dbContext.Users.FindAsync(userId);

                if (userToDelete != null)
                {
                    // Удалите пользователя из DbSet<User> и сохраните изменения
                    dbContext.Users.Remove(userToDelete);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("User deleted successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("User not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid user ID format.");
            }
        }

        public static async Task GetUserAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            if (context.Request.RouteValues["userId"] is string userIdStr && int.TryParse(userIdStr, out int userId))
            {
                // Найдите пользователя в базе данных
                var user = await dbContext.Users.FindAsync(userId);

                if (user != null)
                {
                    // Сериализуйте пользователя в JSON и верните его клиенту
                    var userJson = JsonSerializer.Serialize(user);
                    context.Response.StatusCode = 200; // OK
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(userJson);
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("User not found.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid user ID format.");
            }
        }
        public static async Task<string> GetAllUsersAsync()
        {
            using var dbContext = new MarketplaceDbContext();
            {
                var users = dbContext.Users.ToList();

                // Преобразуем список пользователей в JSON
                var userJsonArray = JsonSerializer.Serialize(users);
                await Console.Out.WriteLineAsync(userJsonArray);
                return userJsonArray;
            }
        }



        public static async Task UpdateUserAsync(HttpContext context, MarketplaceDbContext dbContext)
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();
            // Получите идентификатор пользователя из URL
            if (context.Request.RouteValues["userId"] is string userIdStr && int.TryParse(userIdStr, out int userId))
            {
                // Читайте JSON-тело запроса, как в предыдущем коде

                try
                {
                    // Десериализуйте JSON в объект User и обновите данные пользователя
                    var userToUpdate = await dbContext.Users.FindAsync(userId);
                    if (userToUpdate != null)
                    {
                        var updatedUser = JsonSerializer.Deserialize<User>(jsonBody);
                        userToUpdate.FirstName = updatedUser.FirstName;
                        userToUpdate.LastName = updatedUser.LastName;
                        userToUpdate.Email = updatedUser.Email;
                        userToUpdate.PasswordHash = updatedUser.PasswordHash;
                        userToUpdate.RoleId = updatedUser.RoleId;

                        // Сохраните изменения в базе данных
                        await dbContext.SaveChangesAsync();
                        context.Response.StatusCode = 200; // OK
                        await context.Response.WriteAsync("User updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404; // Not Found
                        await context.Response.WriteAsync("User not found.");
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
                await context.Response.WriteAsync("Invalid user ID format.");
            }
        }
    }
}
