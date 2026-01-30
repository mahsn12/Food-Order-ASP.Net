using System;
using System.Collections.Generic;
using System.Linq;
using FoodDelivery.Domain.Enums;

using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Order> Orders { get; set; }
        public ICollection<Rating> Ratings { get; set; }
    }

}
