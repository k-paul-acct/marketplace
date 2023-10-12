using Marketplace.Api.API_Handlers;
using Marketplace.Api.Models;
using Microsoft.EntityFrameworkCore;
using Marketplace.Api.Data;
using Marketplace.Api.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MarketplaceDbContext>(o => o.UseSqlServer(builder.Configuration["ConnectionStrings:Marketplace"]));

var app = builder.Build();

// Users
app.MapGet("/api/user/auth", async (string email, string password, MarketplaceDbContext context) =>
{
    var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email && x .PasswordHash == password);
    return user is null ? Results.Ok(user) : Results.Unauthorized();
});

app.MapPut("/api/user/create", async (User userModel, MarketplaceDbContext context) =>
{
    userModel.FirstName = userModel.FirstName.Trim();
    userModel.LastName = userModel.LastName.Trim();
    userModel.Email = userModel.Email.Trim();
    userModel.PasswordHash = userModel.PasswordHash.Trim();
    userModel.Phone = userModel.Phone.Trim();
    userModel.ImageUrl = userModel.ImageUrl?.Trim();
    userModel.RoleId = Roles.User;

    try
    {
        context.Users.Add(userModel);
        await context.SaveChangesAsync();
        return Results.Json(userModel, statusCode: StatusCodes.Status201Created);
    }
    catch (DbUpdateException)
    {
        return Results.Conflict();
    }
});

app.MapDelete("/api/user/deletebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.DeleteEntityByIDAsync<User>(context, dbContext));

app.MapGet("/api/user/getbyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetEntityAsync<User>(context, dbContext));

app.MapPost("/api/user/updatebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.UpdateEntityAsync<User>(context, dbContext));

app.MapGet("/api/user/getall", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetAllEntitiesAsync<User>(dbContext, context));

app.MapPost("/api/user/getbyfields", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.SearchEntitiesByJsonAsync<User>(context, dbContext));

// Products
app.MapPut("/api/product/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.CreateEntityAsync<Product>(context, dbContext));

app.MapDelete("/api/product/deletebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.DeleteEntityByIDAsync<Product>(context, dbContext));

app.MapGet("/api/product/getbyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetEntityAsync<Product>(context, dbContext));

app.MapPost("/api/product/updatebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.UpdateEntityAsync<Product>(context, dbContext));

app.MapGet("/api/product/getall", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetAllEntitiesAsync<Product>(dbContext, context));

// Orders
app.MapPut("/api/order/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.CreateEntityAsync<Order>(context, dbContext));

app.MapDelete("/api/order/deletebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.DeleteEntityByIDAsync<Order>(context, dbContext));

app.MapGet("/api/order/getbyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetEntityAsync<Order>(context, dbContext));

app.MapPost("/api/order/updatebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.UpdateEntityAsync<Order>(context, dbContext));

app.MapGet("/api/order/getall", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetAllEntitiesAsync<Order>(dbContext, context));

// Reviews
app.MapPut("/api/review/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.CreateEntityAsync<Review>(context, dbContext));

app.MapDelete("/api/review/deletebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.DeleteEntityByIDAsync<Review>(context, dbContext));

app.MapGet("/api/review/getbyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetEntityAsync<Review>(context, dbContext));

app.MapPost("/api/review/updatebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.UpdateEntityAsync<Review>(context, dbContext));

app.MapGet("/api/review/getall", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetAllEntitiesAsync<Review>(dbContext, context));

// Categories
app.MapPut("/api/category/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.CreateEntityAsync<Category>(context, dbContext));

app.MapDelete("/api/category/getbyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.DeleteEntityByIDAsync<Category>(context, dbContext));

app.MapPost("/api/category/updatebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.UpdateEntityAsync<Category>(context, dbContext));

app.MapGet("/api/category/deletebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetEntityAsync<Category>(context, dbContext));

app.MapGet("/api/category/getall", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetAllEntitiesAsync<Category>(dbContext, context));

// Roles
app.MapPut("/api/role/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.CreateEntityAsync<Role>(context, dbContext));

app.MapDelete("/api/role/getbyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.DeleteEntityByIDAsync<Role>(context, dbContext));

app.MapPost("/api/role/updatebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.UpdateEntityAsync<Role>(context, dbContext));

app.MapGet("/api/role/deletebyid/{Id}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetEntityAsync<Role>(context, dbContext));

app.MapGet("/api/role/getall", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await APIHandler.GetAllEntitiesAsync<Role>(dbContext, context));

// DB stuff.
using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
await dbContext.Database.MigrateAsync();



app.Run();