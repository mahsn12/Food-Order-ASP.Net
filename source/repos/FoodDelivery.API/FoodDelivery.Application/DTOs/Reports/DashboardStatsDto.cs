using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Application.DTOs.Reports
{
    public record DashboardStatsDto(
        int TotalOrders,
        decimal TotalRevenue,
        int TotalUsers,
        int TotalRestaurants,
        int PendingOrders
    );
}