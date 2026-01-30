using System;
using System.Collections.Generic;
using System.Linq;
using FoodDelivery.Domain.Enums;

using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionRef { get; set; }
        public DateTime? PaidAt { get; set; }

        public Order Order { get; set; }
    }

}
