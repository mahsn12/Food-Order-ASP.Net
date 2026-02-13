using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Application.DTOs.Restaurants
{
    public record CreateRestaurantDto(
        [Required] string Name,
        string Description,
        [Required] string Phone,
        [Required, EmailAddress] string Email,
        [Required, MinLength(6)] string Password
    );
}
