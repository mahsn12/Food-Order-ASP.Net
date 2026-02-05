using System;
using FoodDelivery.Domain.Enums;

namespace FoodDelivery.Application.DTOs.Users
{
    public record UserDto(
        int Id,
        string Name,
        string Email,
        string Phone,
        UserRole Role,
        DateTime CreatedAt
    );
}