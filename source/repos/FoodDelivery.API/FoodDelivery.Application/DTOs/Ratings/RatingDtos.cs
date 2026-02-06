namespace FoodDelivery.Application.DTOs.Ratings;

public record RatingDto(int Id, int UserId, int OrderId, int RestaurantId, int? DriverId, int RatingValue, string Comment, DateTime CreatedAt);
public record CreateRatingDto(int UserId, int OrderId, int RestaurantId, int? DriverId, int RatingValue, string Comment);
public record UpdateRatingDto(int Id, int UserId, int OrderId, int RestaurantId, int? DriverId, int RatingValue, string Comment);
