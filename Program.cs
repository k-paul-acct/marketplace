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
app.MapPost("/api/user/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await UserAPIHandler.CreateUserAsync(context, dbContext);
});

app.MapPost("/api/user/delete/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await UserAPIHandler.DeleteUserAsync(context, dbContext);
});

app.MapPost("/api/user/get/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await UserAPIHandler.GetUserAsync(context, dbContext);
});

app.MapPost("/api/user/update/{userId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await UserAPIHandler.UpdateUserAsync(context, dbContext);
});
app.MapGet("/api/user/getall", async (HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(await UserAPIHandler.GetAllUsersAsync());
});


// Products
app.MapPost("/api/product/create", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await ProductAPIHandler.CreateProductAsync(context, dbContext);
});

app.MapPost("/api/product/delete/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await ProductAPIHandler.DeleteProductAsync(context, dbContext);
});

app.MapPost("/api/product/get/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await ProductAPIHandler.GetProductAsync(context, dbContext);
});

app.MapPost("/api/product/update/{productId}", async (MarketplaceDbContext dbContext, HttpContext context) =>
{
    await ProductAPIHandler.UpdateProductAsync(context, dbContext);
});
app.MapGet("/api/product/getall", async (HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(await ProductAPIHandler.GetAllProductsAsync());
});

app.Run();