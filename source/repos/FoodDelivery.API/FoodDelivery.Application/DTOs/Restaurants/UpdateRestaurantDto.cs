using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Application.DTOs.Restaurants
{
    public record UpdateRestaurantDto(
        int Id,
        [Required] string Name,
        string Description,
        [Required] string Phone,
        [Required, EmailAddress] string Email,
        bool IsOpen
    );
}
