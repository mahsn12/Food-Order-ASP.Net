using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Application.DTOs.Delivery
{
    public record UpdateDeliveryStatusDto
    {

        public string Status { get; init; } = string.Empty;

    }
}
