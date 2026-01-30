using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Domain.Entities
{
    public class DeliveryTracking
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; }
    }

}
