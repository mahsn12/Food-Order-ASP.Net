using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FoodDelivery.Application.DTOs.Delivery
{
    public record DriverDto(
             int OrderId,
             string RestaurantName,
             string PickupLocation,
             string DropOffLocation,
             string Status,
             DateTime SchedualeTime,
             int DriverId,
             string DriverName
            )
    {
        private string? v;
        private DateTime scheduledTime;

        public DriverDto(int orderId, string pickupLocation, string dropoffLocation)
            : this(orderId, string.Empty, pickupLocation, dropoffLocation, string.Empty, default, 0, string.Empty)
        {
        }

        public DriverDto(int orderId, string pickupLocation, string dropoffLocation, string? v, DateTime scheduledTime) : this(orderId, pickupLocation, dropoffLocation)
        {
            this.v = v;
            this.scheduledTime = scheduledTime;
        }
    }
}
