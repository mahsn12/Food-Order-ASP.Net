namespace FoodDelivery.Application.DTOs.OrderItems;

public record OrderItemDto(int Id, int OrderId, int ProductId, int Quantity, decimal Price);
public record CreateOrderItemDto(int OrderId, int ProductId, int Quantity, decimal Price);
public record UpdateOrderItemDto(int Id, int OrderId, int ProductId, int Quantity, decimal Price);
