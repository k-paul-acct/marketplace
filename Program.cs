using API_Marketplace_.net_7_v1.Controllers;
using API_Marketplace_.net_7_v1.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:8080"); 
builder.Services.AddDbContext<MarketplaceDbContext>(options =>
{
    options.UseSqlServer("Server=DESKTOP-L57VS11;Database=master;Trusted_Connection=True;"); // Замените на вашу строку подключения
});

var app = builder.Build();


// Users
app.MapPut("/api/user/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await UserAPIHandler.CreateUserAsync(context, dbContext));

app.MapDelete("/api/user/deletebyid/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await UserAPIHandler.DeleteUserByIDAsync(context, dbContext));

app.MapGet("/api/user/getbyid/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await UserAPIHandler.GetUserByIDAsync(context, dbContext));

app.MapPost("/api/user/updatebyid/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await UserAPIHandler.UpdateUserAsync(context, dbContext));

app.MapGet("/api/user/getall", async (HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await UserAPIHandler.GetAllUsersAsync()));

app.MapPost("/api/user/auth", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await UserAPIHandler.AuthenticateUserAsync(context, dbContext)));


// Products
app.MapPut("/api/product/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await ProductAPIHandler.CreateProductAsync(context, dbContext));

app.MapDelete("/api/product/deletebyid/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await ProductAPIHandler.DeleteProductByIDAsync(context, dbContext));

app.MapGet("/api/product/getbyid/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await ProductAPIHandler.GetProductAsync(context, dbContext));

app.MapPost("/api/product/updatebyid/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await ProductAPIHandler.UpdateProductAsync(context, dbContext));

app.MapGet("/api/product/getall", async (HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await ProductAPIHandler.GetAllProductsAsync()));


//Orders
app.MapPut("/api/order/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await OrderAPIHandler.CreateOrderAsync(context, dbContext));

app.MapDelete("/api/order/deletebyid/{orderId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await OrderAPIHandler.CreateOrderAsync(context, dbContext));

app.MapGet("/api/order/getbyid/{orderId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await OrderAPIHandler.CreateOrderAsync(context, dbContext));

app.MapPost("/api/order/updatebyid/{orderId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await OrderAPIHandler.CreateOrderAsync(context, dbContext));
app.MapGet("/api/order/getall", async (HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await OrderAPIHandler.GetAllOrdersAsync()));


// OrderItems
app.MapPut("/api/orderitem/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await OrderItemAPIHandler.CreateOrderItemAsync(context, dbContext));

app.MapDelete("/api/orderitem/deletebyid/{orderItemId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await OrderItemAPIHandler.DeleteOrderItemByIdAsync(context, dbContext));

app.MapGet("/api/orderitem/getbyid/{orderItemId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await OrderItemAPIHandler.GetOrderItemAsync(context, dbContext));

app.MapPost("/api/orderitem/updatebyid/{orderItemId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await OrderItemAPIHandler.UpdateOrderItemAsync(context, dbContext));

app.MapGet("/api/orderitem/getall", async (HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await OrderItemAPIHandler.GetAllOrderItemsAsync()));


// Reviews
app.MapPut("/api/review/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await ReviewAPIHandler.CreateReviewAsync(context, dbContext));

app.MapDelete("/api/review/deletebyid/{reviewId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await ReviewAPIHandler.DeleteReviewByIdAsync(context, dbContext));

app.MapGet("/api/review/getbyid/{reviewId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await ReviewAPIHandler.GetReviewAsync(context, dbContext));

app.MapPost("/api/review/updatebyid/{reviewId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await ReviewAPIHandler.UpdateReviewAsync(context, dbContext));

app.MapGet("/api/review/getall", async (HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await ReviewAPIHandler.GetAllReviewsAsync()));


// Categories
app.MapPut("/api/category/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await CategoryAPIHandler.CreateCategoryAsync(context, dbContext));

app.MapDelete("/api/category/getbyid/{categoryId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await CategoryAPIHandler.GetCategoryAsync(context, dbContext));

app.MapPost("/api/category/updatebyid/{categoryId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await CategoryAPIHandler.UpdateCategoryAsync(context, dbContext));

app.MapGet("/api/category/deletebyid/{categoryId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await CategoryAPIHandler.DeleteCategoryByIDAsync(context, dbContext));

app.MapGet("/api/category/getall", async (HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await CategoryAPIHandler.GetAllCategoriesAsync()));

// Wishlists
app.MapPut("/api/wishlist/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await WishlistAPIHandler.CreateWishlistAsync(context, dbContext));

app.MapDelete("/api/wishlist/getbyid/{wishlistId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await WishlistAPIHandler.GetWishlistAsync(context, dbContext));

app.MapPost("/api/wishlist/updatebyid/{wishlistId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await WishlistAPIHandler.UpdateWishlistAsync(context, dbContext));

app.MapGet("/api/wishlist/deletebyid/{wishlistId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await WishlistAPIHandler.DeleteWishlistByIDAsync(context, dbContext));

app.MapGet("/api/wishlist/getall", async (HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await WishlistAPIHandler.GetAllWishlistsAsync()));


// Roles
app.MapPut("/api/role/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await RoleAPIHandler.CreateRoleAsync(context, dbContext));

app.MapDelete("/api/role/getbyid/{roleId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await RoleAPIHandler.GetRoleAsync(context, dbContext));

app.MapPost("/api/role/updatebyid/{roleId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await RoleAPIHandler.UpdateRoleAsync(context, dbContext));

app.MapGet("/api/role/deletebyid/{roleId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
    await RoleAPIHandler.DeleteRoleByIDAsync(context, dbContext));

app.MapGet("/api/role/getall", async (HttpContext context) =>
    await context.Response.WriteAsJsonAsync(await RoleAPIHandler.GetAllRolesAsync()));


app.Run();