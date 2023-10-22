using Marketplace.Api.Data;
using Marketplace.Api.Data.Models;
using Marketplace.Api.Dto;
using Marketplace.Api.Mapping;
using Marketplace.Api.Types;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services configuration.
builder.Services.AddDbContext<MarketplaceDbContext>(o => o.UseSqlServer(builder.Configuration["ConnectionStrings:Marketplace"]));

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
    var user = await context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == password);
    return user is null ? Results.Unauthorized() : Results.Ok(user.MapToDto());
});

userApi.MapPut("/create", async (UserDto userModel, MarketplaceDbContext context) =>
{
    var role = await context.Roles.FirstAsync(x => x.Id == (int)Roles.User);
    var user = new User
    {
        FirstName = userModel.FirstName,
        LastName = userModel.LastName,
        Email = userModel.Email,
        PasswordHash = userModel.PasswordHash,
        Phone = userModel.Phone,
        ImageUrl = userModel.ImageUrl,
        Roles = { role, },
    };

    try
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return Results.Json(user.MapToDto(), statusCode: StatusCodes.Status201Created);
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
    var user = await context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);
    return user is null ? Results.NotFound() : Results.Ok(user.MapToDto());
});

userApi.MapPost("/update", async (UserDto userModel, MarketplaceDbContext context) =>
{
    var user = await context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == userModel.Id);
    if (user is null) return Results.NotFound();

    user.FirstName = userModel.FirstName;
    user.LastName = userModel.LastName;
    user.Email = userModel.Email;
    user.PasswordHash = userModel.PasswordHash;
    user.Phone = userModel.Phone;
    user.ImageUrl = userModel.ImageUrl;

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(user.MapToDto());
    }
    catch (DbUpdateException e)
    {
        Console.WriteLine(e);
        return Results.Conflict();
    }
});

userApi.MapGet("/getall", async (MarketplaceDbContext context) =>
{
    var users = await context.Users.Include(x => x.Roles).ToListAsync();
    return Results.Ok(users.Select(x => x.MapToDto()));
});

userApi.MapGet("/products", async (int id, MarketplaceDbContext context) =>
{
    var products = await context.Products
        .Include(x => x.Users).ThenInclude(x => x.Roles)
        .Include(x => x.Categories)
        .Where(x => x.Users.Any(y => y.Id == id))
        .ToListAsync();
    return Results.Ok(products.Select(x => x.MapToDto()));
});

// Product.
productApi.MapPut("/create", async (ProductDto productModel, MarketplaceDbContext context) =>
{
    var seller = await context.Users.Include(x => x.Roles).FirstAsync(x => x.Id == productModel.SellerId);
    var category = await context.Categories.FindAsync(productModel.CategoryId);
    var product = new Product
    {
        Name = productModel.Name,
        Description = productModel.Description,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = null,
        ImageUrl = productModel.ImageUrl,
        Price = productModel.Price,
        StockQuantity = productModel.StockQuantity,
        Categories = { category!, },
        Users = { seller, },
    };

    try
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return Results.Json(product.MapToDto(), statusCode: StatusCodes.Status200OK);
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
    var product = await context.Products
        .Include(x => x.Categories)
        .Include(x => x.Users).ThenInclude(x => x.Roles)
        .FirstOrDefaultAsync(x => x.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product.MapToDto());
});

productApi.MapGet("/{id:int}/reviews", async (int id, MarketplaceDbContext context) =>
{
    var reviews = await context.Reviews
        .Include(x => x.Product).ThenInclude(x => x!.Categories)
        .Include(x => x.User).ThenInclude(x => x!.Roles)
        .Where(x => x.ProductId == id)
        .ToListAsync();
    return Results.Ok(reviews.Select(x => x.MapToDto()));
});

productApi.MapPost("/update", async (ProductDto productModel, MarketplaceDbContext context) =>
{
    var category = await context.Categories.FindAsync(productModel.CategoryId);
    var product = await context.Products
        .Include(x => x.Categories)
        .Include(x => x.Users).ThenInclude(x => x.Roles)
        .FirstOrDefaultAsync(x => x.Id == productModel.Id);
    if (product is null) return Results.NotFound();

    product.Name = productModel.Name.Trim();
    product.Description = productModel.Description?.Trim();
    product.Price = productModel.Price;
    product.StockQuantity = productModel.StockQuantity;
    product.ImageUrl = productModel.ImageUrl?.Trim();
    product.Categories = new List<Category> { category!, };

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(product.MapToDto());
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
        .Include(x => x.Categories)
        .Include(x => x.Users).ThenInclude(x => x.Roles)
        .ToListAsync();
    return Results.Ok(products.Select(x => x.MapToDto()));
});

// Order.
orderApi.MapPut("/create", async (OrderDto orderModel, MarketplaceDbContext context) =>
{
    var order = new Order
    {
        UserId = orderModel.UserId,
        ProductId = orderModel.ProductId,
        TotalQuantity = orderModel.TotalQuantity,
        TotalAmount = orderModel.TotalAmount,
        CreateTime = DateTime.UtcNow,
    };

    try
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return Results.Json(order.MapToDto(), statusCode: StatusCodes.Status201Created);
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
    return order is null ? Results.NotFound() : Results.Ok(order.MapToDto());
});

orderApi.MapPost("/update", async (OrderDto orderModel, MarketplaceDbContext context) =>
{
    var order = await context.Orders.FindAsync(orderModel.Id);
    if (order is null) return Results.NotFound();

    order.TotalQuantity = orderModel.TotalQuantity;
    order.TotalAmount = orderModel.TotalAmount;

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(order.MapToDto());
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
    return Results.Ok(orders.Select(x => x.MapToDto()));
});

// Review.
reviewApi.MapPut("/create", async (ReviewDto reviewModel, MarketplaceDbContext context) =>
{
    var review = new Review
    {
        CreatedAt = DateTime.UtcNow,
        Comment = reviewModel.Comment,
        ImageUrl = reviewModel.ImageUrl,
        ProductId = reviewModel.ProductId,
        UserId = reviewModel.UserId,
        Rating = reviewModel.Rating,
    };

    try
    {
        context.Reviews.Add(review);
        await context.SaveChangesAsync();
        return Results.Json(review.MapToDto(), statusCode: StatusCodes.Status201Created);
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
    return review is null ? Results.NotFound() : Results.Ok(review.MapToDto());
});

reviewApi.MapPost("/update", async (ReviewDto reviewModel, MarketplaceDbContext context) =>
{
    var review = await context.Reviews.FindAsync(reviewModel.Id);
    if (review is null) return Results.NotFound();

    review.Comment = reviewModel.Comment;
    review.ImageUrl = reviewModel.ImageUrl;

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(review.MapToDto());
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
    return Results.Ok(reviews.Select(x => x.MapToDto()));
});

// Category.
categoryApi.MapPut("/create", async (CategoryDto categoryModel, MarketplaceDbContext context) =>
{
    var category = new Category
    {
        Name = categoryModel.Name,
        Description = categoryModel.Description,
    };

    try
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return Results.Ok(category.MapToDto());
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
    return category is null ? Results.NotFound() : Results.Ok(category.MapToDto());
});

categoryApi.MapPost("/update", async (CategoryDto categoryModel, MarketplaceDbContext context) =>
{
    var category = await context.Categories.FindAsync(categoryModel.Id);
    if (category is null) return Results.NotFound();

    category.Name = categoryModel.Name;
    category.Description = categoryModel.Description;

    try
    {
        await context.SaveChangesAsync();
        return Results.Ok(category.MapToDto());
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
    return Results.Ok(categories.Select(x => x.MapToDto()));
});

// Cart.
cartApi.MapPost("/add-product", async (int userId, int productId, MarketplaceDbContext context) =>
{
    var user = await context.Users.Include(x => x.ProductsNavigation).FirstOrDefaultAsync(x => x.Id == userId);
    var product = await context.Products.FindAsync(productId);
    if (user is null || product is null) return Results.NotFound();

    try
    {
        user.ProductsNavigation.Add(product);
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
    var user = await context.Users.Include(x => x.ProductsNavigation).FirstOrDefaultAsync(x => x.Id == userId);
    var product = await context.Products.FindAsync(productId);
    if (user is null || product is null) return Results.NotFound();

    try
    {
        user.ProductsNavigation.Remove(product);
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
    var user = await context.Users
        .Include(x => x.ProductsNavigation).ThenInclude(x => x.Categories)
        .Include(x => x.ProductsNavigation).ThenInclude(x => x.Users).ThenInclude(x => x.Roles)
        .FirstOrDefaultAsync(x => x.Id == userId);
    return user is null ? Results.NotFound() : Results.Ok(user.ProductsNavigation.Select(x => x.MapToDto()));
});

// DB stuff.
using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
try
{
    await dbContext.Database.EnsureCreatedAsync();
}
catch
{
    // ignored
}

// Starting the app.
app.Run();