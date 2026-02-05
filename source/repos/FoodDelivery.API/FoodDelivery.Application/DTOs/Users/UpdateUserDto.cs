using FoodDelivery.Domain.Enums;

namespace FoodDelivery.Application.DTOs.Users
{
    public record UpdateUserDto(
        int Id,
        string Name,
        string Phone,
        UserRole Role // الآدمن يقدر يغير صلاحيات المستخدم
    );
}