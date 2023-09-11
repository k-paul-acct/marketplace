using API_Marketplace_.net_7_v1.Models;
using System.Text.Json;

public class RoleAPIHandler
{
    public static async Task CreateRoleAsync(HttpContext context, MarketplaceDbContext dbContext)
    {
        // Читаем JSON-тело запроса
        using var reader = new StreamReader(context.Request.Body);
        var jsonBody = await reader.ReadToEndAsync();

        try
        {
            // Десериализуем JSON в объект Role
            var newRole = JsonSerializer.Deserialize<Role>(jsonBody);

            // Добавляем роль в DbSet<Role> и сохраняем изменения в базе данных
            dbContext.Roles.Add(newRole);
            await dbContext.SaveChangesAsync();

            context.Response.StatusCode = 201; // Created
            await context.Response.WriteAsync("Role created successfully.");
        }
        catch (JsonException)
        {
            context.Response.StatusCode = 400; // Bad Request
            await context.Response.WriteAsync("Invalid JSON data.");
        }
    }

    public static async Task GetRoleAsync(HttpContext context, MarketplaceDbContext dbContext)
    {
        if (context.Request.RouteValues["RoleId"] is string roleIdStr && int.TryParse(roleIdStr, out int roleId))
        {
            // Найдите роль в базе данных по RoleId
            var role = await dbContext.Roles.FindAsync(roleId);

            if (role != null)
            {
                // Сериализуйте роль в JSON и верните ее клиенту
                var roleJson = JsonSerializer.Serialize(role);
                context.Response.StatusCode = 200; // OK
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(roleJson);
            }
            else
            {
                context.Response.StatusCode = 404; // Not Found
                await context.Response.WriteAsync("Role not found.");
            }
        }
        else
        {
            context.Response.StatusCode = 400; // Bad Request
            await context.Response.WriteAsync("Invalid Role ID format.");
        }
    }

    public static async Task UpdateRoleAsync(HttpContext context, MarketplaceDbContext dbContext)
    {
        // Получите RoleId из URL
        if (context.Request.RouteValues["RoleId"] is string roleIdStr && int.TryParse(roleIdStr, out int roleId))
        {
            // Читаем JSON-тело запроса
            using var reader = new StreamReader(context.Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            try
            {
                // Десериализуем JSON в объект Role и обновляем данные роли
                var roleToUpdate = await dbContext.Roles.FindAsync(roleId);
                if (roleToUpdate != null)
                {
                    var updatedRole = JsonSerializer.Deserialize<Role>(jsonBody);
                    roleToUpdate.RoleName = updatedRole.RoleName;

                    // Сохраняем изменения в базе данных
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("Role updated successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Role not found.");
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
            await context.Response.WriteAsync("Invalid Role ID format.");
        }
    }

    public static async Task DeleteRoleByIDAsync(HttpContext context, MarketplaceDbContext dbContext)
    {
        if (context.Request.RouteValues["RoleId"] is string roleIdStr && int.TryParse(roleIdStr, out int roleId))
        {
            // Найдите роль в базе данных по RoleId
            var roleToDelete = await dbContext.Roles.FindAsync(roleId);

            if (roleToDelete != null)
            {
                // Удаляем роль из DbSet<Role> и сохраняем изменения
                dbContext.Roles.Remove(roleToDelete);
                await dbContext.SaveChangesAsync();
                context.Response.StatusCode = 200; // OK
                await context.Response.WriteAsync("Role deleted successfully.");
            }
            else
            {
                context.Response.StatusCode = 404; // Not Found
                await context.Response.WriteAsync("Role not found.");
            }
        }
        else
        {
            context.Response.StatusCode = 400; // Bad Request
            await context.Response.WriteAsync("Invalid Role ID format.");
        }
    }

    public static async Task<string> GetAllRolesAsync()
    {
        using var dbContext = new MarketplaceDbContext();
        {
            var roles = dbContext.Roles.ToList();

            // Преобразуем список ролей в JSON
            var roleJsonArray = JsonSerializer.Serialize(roles);
            await Console.Out.WriteLineAsync(roleJsonArray);
            return roleJsonArray;
        }
    }
}
