using FoodDelivery.Domain.Enums;

namespace FoodDelivery.Application.DTOs.Orders;

public record OrderDto(
    int Id,
    int UserId,
    int RestaurantId,
    int? DriverId,
    decimal TotalPrice,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime? DeliveredAt);

public record CreateOrderDto(int UserId, int RestaurantId, int? DriverId, decimal TotalPrice, OrderStatus Status);

public record UpdateOrderDto(int Id, int UserId, int RestaurantId, int? DriverId, decimal TotalPrice, OrderStatus Status, DateTime? DeliveredAt);
