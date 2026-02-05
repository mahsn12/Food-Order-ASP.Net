using System.ComponentModel.DataAnnotations;
using FoodDelivery.Domain.Enums;

namespace FoodDelivery.Application.DTOs.Users
{
    public record CreateUserDto(
        [Required] string Name,
        [Required, EmailAddress] string Email,
        [Required] string Phone,
        [Required] string Password,
        [Required] UserRole Role 
    );
}