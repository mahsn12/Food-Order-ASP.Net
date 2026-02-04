using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Application.DTOs.Restaurants
{
    public record CreateRestaurantDto(
        [Required] string Name,
        string Description,
        [Required] string Phone
    );
}