namespace FoodDelivery.Application.DTOs.Restaurants
{
    public record RestaurantDto(
        int Id,
        string Name,
        string Description,
        string Phone,
        string Email,
        double RatingAvg,
        bool IsOpen,
        DateTime CreatedAt
    );
}
