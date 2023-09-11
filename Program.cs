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
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;"); // Замените на вашу строку подключения
});

var app = builder.Build();


// Users
app.MapPost("/api/user/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await UserAPIHandler.CreateUserAsync(context, dbContext);
});

app.MapGet("/api/user/deletebyid/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await UserAPIHandler.DeleteUserByIDAsync(context, dbContext);
});

app.MapGet("/api/user/getbyid/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await UserAPIHandler.GetUserByIDAsync(context, dbContext);
});

app.MapPost("/api/user/updatebyid/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await UserAPIHandler.UpdateUserAsync(context, dbContext);
});
app.MapGet("/api/user/getall", async (HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(await UserAPIHandler.GetAllUsersAsync());
});
app.MapPost("/api/user/auth", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(await UserAPIHandler.AuthenticateUserAsync(context, dbContext));
});


// Products
app.MapPost("/api/product/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await ProductAPIHandler.CreateProductAsync(context, dbContext);
});

app.MapGet("/api/product/deletebyid/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await ProductAPIHandler.DeleteProductByIDAsync(context, dbContext);
});

app.MapGet("/api/product/getbyid/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await ProductAPIHandler.GetProductAsync(context, dbContext);
});

app.MapPost("/api/product/updatebyid/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await ProductAPIHandler.UpdateProductAsync(context, dbContext);
});
app.MapGet("/api/product/getall", async (HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(await ProductAPIHandler.GetAllProductsAsync());
});


//Orders
app.MapPost("/api/order/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await OrderAPIHandler.CreateOrderAsync(context, dbContext);
});

app.MapGet("/api/order/deletebyid/{orderId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await OrderAPIHandler.CreateOrderAsync(context, dbContext);
});

app.MapGet("/api/order/getbyid/{orderId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await OrderAPIHandler.CreateOrderAsync(context, dbContext);
});

app.MapPost("/api/order/updatebyid/{orderId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await OrderAPIHandler.CreateOrderAsync(context, dbContext);
});
app.MapGet("/api/order/getall", async (HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(await OrderAPIHandler.GetAllOrdersAsync());
});

app.Run();