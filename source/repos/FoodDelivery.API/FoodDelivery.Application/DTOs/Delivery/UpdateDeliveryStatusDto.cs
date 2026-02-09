using System.ComponentModel.DataAnnotations;
using FoodDelivery.Domain.Enums;

namespace FoodDelivery.Application.DTOs.Driver
{
    public record UpdateDeliveryStatusDto(
        [Required] DeliveryStatus Status
    );
}