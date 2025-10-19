namespace RestaurantAPI.DTOs;

public record OrderItemDto(int MenuItemId, int Quantity);
public record OrderDto(string CustomerName, List<OrderItemDto> Items);
public record UpdateOrderStatusDto(string Status);
