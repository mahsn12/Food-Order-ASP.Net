using System;
using System.Collections.Generic;

namespace FoodDelivery.Domain.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? IdentityUserId { get; set; }
        public double RatingAvg { get; set; }
        public bool IsOpen { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Product> Products { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
