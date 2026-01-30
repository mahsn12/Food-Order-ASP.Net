using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,      
        Preparing = 2,    
        OnTheWay = 3,    
        Delivered = 4,   
        Cancelled = 5
    }
}
