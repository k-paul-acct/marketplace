using Marketplace.Api.Data;
using Marketplace.Api.Models;
using Marketplace.Api.Types;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services configuration.
builder.Services.AddDbContext<MarketplaceDbContext>(o =>
    o.UseSqlServer(builder.Configuration["ConnectionStrings:Marketplace"]));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// App configuration.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var api = app.MapGroup("/api");
var userApi = api.MapGroup("/user").WithTags("User");
var productApi = api.MapGroup("/product").WithTags("Product");
var orderApi = api.MapGroup("/order").WithTags("Order");
var reviewApi = api.MapGroup("/review").WithTags("Review");
var categoryApi = api.MapGroup("/category").WithTags("Category");
var cartApi = api.MapGroup("/cart").WithTags("Cart");

// Users.
userApi.MapGet("/auth", async (string email, string password, MarketplaceDbContext context) =>
{
    var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == password);
    return user is null ? Results.Unauthorized() : Results.Ok(user);
});

userApi.MapPut("/create", async (User userModel, MarketplaceDbContext context) =>
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
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

userApi.MapDelete("/deletebyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var user = await context.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    try
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

userApi.MapGet("/getbyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var user = await context.Users.FindAsync(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

userApi.MapPost("/update", async (User userModel, MarketplaceDbContext context) =>
{
    var user = await context.Users.FindAsync(userModel.UserId);
    if (user is null) return Results.NotFound();

    user.FirstName = userModel.FirstName.Trim();
    user.LastName = userModel.LastName.Trim();
    user.Email = userModel.Email.Trim();
    user.PasswordHash = userModel.PasswordHash.Trim();
    user.Phone = userModel.Phone.Trim();
    user.ImageUrl = userModel.ImageUrl?.Trim();

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(user);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

userApi.MapGet("/getall", async (MarketplaceDbContext context) =>
{
    var users = await context.Users.ToListAsync();
    return Results.Ok(users);
});

userApi.MapGet("/products", async (int id, MarketplaceDbContext context) =>
{
    var products = await context.Products.Include(x => x.Seller).Where(x => x.SellerId == id).ToListAsync();
    return Results.Ok(products);
});

// Product.
productApi.MapPut("/create", async (Product productModel, MarketplaceDbContext context) =>
{
    productModel.Name = productModel.Name.Trim();
    productModel.Description = productModel.Description?.Trim();
    productModel.CreatedAt = DateTime.UtcNow;
    productModel.UpdatedAt = null;
    productModel.ImageUrl = productModel.ImageUrl?.Trim();

    try
    {
        context.Products.Add(productModel);
        await context.SaveChangesAsync();
        return Results.Json(productModel, statusCode: StatusCodes.Status200OK);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.BadRequest();
    }
});

productApi.MapDelete("/deletebyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var product = await context.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    try
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

productApi.MapGet("/getbyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var product = await context.Products.FindAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

productApi.MapGet("/{id:int}/reviews", async (int id, MarketplaceDbContext context) =>
{
    var reviews = await context.Reviews.Where(x => x.ProductId == id).ToListAsync();
    return Results.Ok(reviews);
});

productApi.MapPost("/update", async (Product productModel, MarketplaceDbContext context) =>
{
    var product = await context.Products.FindAsync(productModel.ProductId);
    if (product is null) return Results.NotFound();

    product.Name = productModel.Name.Trim();
    product.Description = productModel.Description?.Trim();
    product.Price = productModel.Price;
    product.StockQuantity = productModel.StockQuantity;
    product.ImageUrl = productModel.ImageUrl?.Trim();
    product.CategoryId = productModel.CategoryId;

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(product);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

productApi.MapGet("/getall", async (MarketplaceDbContext context) =>
{
    var products = await context.Products
        .Where(x => x.UpdatedAt != null)
        .Include(x => x.Category)
        .Include(x => x.Seller)
        .ToListAsync();
    return Results.Ok(products);
});

// Order.
orderApi.MapPut("/create", async (Order orderModel, MarketplaceDbContext context) =>
{
    orderModel.OrderDate = DateTime.UtcNow;

    try
    {
        context.Orders.Add(orderModel);
        await context.SaveChangesAsync();
        return Results.Json(orderModel, statusCode: StatusCodes.Status201Created);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict(e);
    }
});

orderApi.MapDelete("/deletebyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var order = await context.Orders.FindAsync(id);
    if (order is null) return Results.NotFound();

    try
    {
        context.Orders.Remove(order);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

orderApi.MapGet("/getbyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var order = await context.Orders.FindAsync(id);
    return order is null ? Results.NotFound() : Results.Ok(order);
});

orderApi.MapPost("/update", async (Order orderModel, MarketplaceDbContext context) =>
{
    var order = await context.Orders.FindAsync(orderModel.OrderId);
    if (order is null) return Results.NotFound();

    order.TotalQuantity = orderModel.TotalQuantity;
    order.TotalAmount = orderModel.TotalAmount;

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(order);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

orderApi.MapGet("/getall", async (int userId, MarketplaceDbContext context) =>
{
    var orders = await context.Orders.Where(x => x.UserId == userId).ToListAsync();
    return Results.Ok(orders);
});

// Review.
reviewApi.MapPut("/create", async (Review reviewModel, MarketplaceDbContext context) =>
{
    reviewModel.CreatedAt = DateTime.UtcNow;
    reviewModel.Comment = reviewModel.Comment.Trim();
    reviewModel.ImageUrl = reviewModel.ImageUrl?.Trim();

    try
    {
        context.Reviews.Add(reviewModel);
        await context.SaveChangesAsync();
        return Results.Json(reviewModel, statusCode: StatusCodes.Status201Created);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

reviewApi.MapDelete("/deletebyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var review = await context.Reviews.FindAsync(id);
    if (review is null) return Results.NotFound();

    try
    {
        context.Reviews.Remove(review);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

reviewApi.MapGet("/getbyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var review = await context.Reviews.FindAsync(id);
    return review is null ? Results.NotFound() : Results.Ok(review);
});

reviewApi.MapPost("/update", async (Review reviewModel, MarketplaceDbContext context) =>
{
    var review = await context.Reviews.FindAsync(reviewModel.ReviewId);
    if (review is null) return Results.NotFound();

    review.Comment = reviewModel.Comment.Trim();
    review.ImageUrl = reviewModel.ImageUrl?.Trim();

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(review);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

reviewApi.MapGet("/getall", async (MarketplaceDbContext context) =>
{
    var reviews = await context.Reviews.ToListAsync();
    return Results.Ok(reviews);
});

// Category.
categoryApi.MapPut("/create", async (Category categoryModel, MarketplaceDbContext context) =>
{
    categoryModel.Name = categoryModel.Name.Trim();
    categoryModel.Description = categoryModel.Description?.Trim();

    try
    {
        context.Categories.Add(categoryModel);
        await context.SaveChangesAsync();
        return Results.Ok(context);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

categoryApi.MapDelete("/getbyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var category = await context.Categories.FindAsync(id);
    return category is null ? Results.NotFound() : Results.Ok(category);
});

categoryApi.MapPost("/update", async (Category categoryModel, MarketplaceDbContext context) =>
{
    var category = await context.Categories.FindAsync(categoryModel.CategoryId);
    if (category is null) return Results.NotFound();

    category.Name = categoryModel.Name.Trim();
    category.Description = categoryModel.Description?.Trim();

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(category);
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

categoryApi.MapGet("/deletebyid/{id:int}", async (int id, MarketplaceDbContext context) =>
{
    var category = await context.Categories.FindAsync(id);
    if (category is null) return Results.NotFound();

    try
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

categoryApi.MapGet("/getall", async (MarketplaceDbContext context) =>
{
    var categories = await context.Categories.ToListAsync();
    return Results.Ok(categories);
});

// Cart.
cartApi.MapPost("/add-product", async (int userId, int productId, MarketplaceDbContext context) =>
{
    var item = new UserHasProductInWishlist
    {
        UserId = userId,
        ProductId = productId,
    };

    try
    {
        context.UserHasProductInWishlist.Add(item);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

cartApi.MapPost("/remove-product", async (int userId, int productId, MarketplaceDbContext context) =>
{
    var item = await context.UserHasProductInWishlist
        .Where(x => x.UserId == userId && x.ProductId == productId)
        .FirstOrDefaultAsync();
    if (item is null) return Results.NotFound();

    try
    {
        context.UserHasProductInWishlist.Remove(item);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

cartApi.MapGet("", async (int userId, MarketplaceDbContext context) =>
{
    var products = await (from cartItem in context.UserHasProductInWishlist
        join product in context.Products on cartItem.ProductId equals product.ProductId
        where cartItem.UserId == userId
        select product).ToListAsync();

    return Results.Ok(products);
});

// DB stuff.
using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
try
{
    await dbContext.Database.MigrateAsync();
}
catch
{
    // ignored
}

// Starting the app.
app.Run();