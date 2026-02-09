namespace FoodDelivery.Application.DTOs.Driver
{
    public record DriverDeliveryDto(
        int OrderId,
        string RestaurantName,    
        string CustomerName,      
        string CustomerPhone,     
        decimal TotalPrice,       
        string Status,
        DateTime CreatedAt
    );
}