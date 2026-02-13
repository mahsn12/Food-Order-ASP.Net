using FoodDelivery.Domain.Enums;

namespace FoodDelivery.Application.DTOs.UserPortal;

public record UserProfileDto(int UserId, string Name, string Email, string Phone);

public record UserRestaurantListDto(int Id, string Name, string Description, string Phone, double RatingAvg, bool IsOpen);

public record UserProductDto(int Id, int RestaurantId, string Name, string Description, decimal Price, string ImageUrl, bool IsAvailable);

public record PlaceOrderItemRequest(int ProductId, int Quantity);

public record PlaceOrderRequest(int RestaurantId, List<PlaceOrderItemRequest> Items, PaymentMethod PaymentMethod);

public record UserOrderItemDto(int Id, int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);

public record UserDeliveryTrackingDto(double Latitude, double Longitude, DateTime UpdatedAt);

public record UserPaymentDto(PaymentMethod Method, PaymentStatus Status, string TransactionRef, DateTime? PaidAt);

public record UserOrderSummaryDto(int Id, int RestaurantId, string RestaurantName, decimal TotalPrice, OrderStatus Status, DateTime CreatedAt, DateTime? DeliveredAt);

public record UserOrderDetailsDto(
    int Id,
    int UserId,
    int RestaurantId,
    string RestaurantName,
    decimal TotalPrice,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime? DeliveredAt,
    IReadOnlyList<UserOrderItemDto> Items,
    UserPaymentDto? Payment,
    IReadOnlyList<UserDeliveryTrackingDto> DeliveryTracking);
