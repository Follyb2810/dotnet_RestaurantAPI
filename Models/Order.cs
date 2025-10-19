namespace RestaurantAPI.Models;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = null!;
    public List<OrderItem> Items { get; set; } = new();
    public string Status { get; set; } = "Pending"; // Pending, Preparing, Delivered
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class OrderItem
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
    public int Quantity { get; set; }
}
