using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FoodDelivery.Application.DTOs.Restaurants
{
    public record RestaurantDto(
        int Id,
        string Name,
        string Description,
        string Phone,
        double RatingAvg,
        bool IsOpen,
        DateTime CreatedAt
    );
}