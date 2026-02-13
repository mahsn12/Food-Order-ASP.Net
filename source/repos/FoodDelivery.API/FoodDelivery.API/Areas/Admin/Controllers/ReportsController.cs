using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using FoodDelivery.Domain.Enums;
using FoodDelivery.Application.DTOs.Reports; 

namespace FoodDelivery.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Restaurant> _restaurantRepo;

        public ReportsController(
            IRepository<Order> orderRepo,
            IRepository<User> userRepo,
            IRepository<Restaurant> restaurantRepo)
        {
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _restaurantRepo = restaurantRepo;
        }

        
        [HttpGet("Dashboard")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var orders = await _orderRepo.GetAsync();
            var users = await _userRepo.GetAsync();
            var restaurants = await _restaurantRepo.GetAsync();

            var stats = new DashboardStatsDto(
                TotalOrders: orders.Count(),
                TotalRevenue: orders.Sum(o => o.TotalPrice), 
                TotalUsers: users.Count(),
                TotalRestaurants: restaurants.Count(),
                PendingOrders: orders.Count(o => o.Status == OrderStatus.Pending) 
            );

            return Ok(stats);
        }
    }
}