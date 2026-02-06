namespace FoodDelivery.Application.DTOs.DeliveryTrackings;

public record DeliveryTrackingDto(int Id, int OrderId, double Latitude, double Longitude, DateTime UpdatedAt);
public record CreateDeliveryTrackingDto(int OrderId, double Latitude, double Longitude, DateTime? UpdatedAt = null);
public record UpdateDeliveryTrackingDto(int Id, int OrderId, double Latitude, double Longitude, DateTime? UpdatedAt = null);
