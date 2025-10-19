using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is migrated
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// --- Menu Endpoints ---
app.MapGet("/api/menu", async (AppDbContext db) =>
{
    var menu = await db.MenuItems.ToListAsync();
    return Results.Ok(menu);
});

app.MapPost("/api/menu", async (MenuItemDto dto, AppDbContext db) =>
{
    var menuItem = new MenuItem
    {
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price
    };

    db.MenuItems.Add(menuItem);
    await db.SaveChangesAsync();
    return Results.Created($"/api/menu/{menuItem.Id}", menuItem);
});

// --- Orders Endpoints ---
app.MapPost("/api/orders", async (OrderDto dto, AppDbContext db) =>
{
    var order = new Order
    {
        CustomerName = dto.CustomerName,
        Items = dto.Items.Select(i => new OrderItem
        {
            MenuItemId = i.MenuItemId,
            Quantity = i.Quantity
        }).ToList()
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();
    return Results.Created($"/api/orders/{order.Id}", order);
});

app.MapGet("/api/orders/{id}", async (int id, AppDbContext db) =>
{
    var order = await db.Orders
        .Include(o => o.Items)
        .ThenInclude(i => i.MenuItem)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order == null) return Results.NotFound();
    return Results.Ok(order);
});

app.MapPut("/api/orders/{id}/status", async (int id, UpdateOrderStatusDto dto, AppDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    if (order == null) return Results.NotFound();

    order.Status = dto.Status;
    await db.SaveChangesAsync();
    return Results.Ok(order);
});

app.Run();
