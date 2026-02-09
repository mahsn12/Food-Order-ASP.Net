using FoodDelivery.Application.DTOs.Driver;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.Enums;
using FoodDelivery.Domain.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDelivery.API.Areas.Driver.Controllers
{
    [Area("Driver")]
    [ApiController]
    [Route("api/driver/deliveries")]
    [Authorize(Roles = "Delivery")] 
    public class DeliveryController : ControllerBase
    {
       
        private readonly IRepository<Order> _orderRepo;

        public DeliveryController(IRepository<Order> orderRepo)
        {
            _orderRepo = orderRepo;
        }

       
        private int GetCurrentDriverId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) throw new UnauthorizedAccessException("User ID not found in token");
            return int.Parse(claim.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriverDeliveryDto>>> GetAssignedDeliveries()
        {
            var driverId = GetCurrentDriverId();

           
            var orders = await _orderRepo.GetAsync(
                expression: o => o.DriverId == driverId && o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled,
                includes: new System.Linq.Expressions.Expression<Func<Order, object>>[]
                {
                    o => o.Restaurant, 
                    o => o.User        
                }
            );

            var result = orders.Select(o => new DriverDeliveryDto(
                o.Id,
                o.Restaurant.Name,
                o.User.Name,
                o.User.Phone,
                o.TotalPrice,
                o.Status.ToString(),
                o.CreatedAt
            ));

            return Ok(result);
        }

       
        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<DriverDeliveryDto>> GetDeliveryDetails(int orderId)
        {
            var driverId = GetCurrentDriverId();

            var order = await _orderRepo.GetOneAsync(
                expression: o => o.Id == orderId && o.DriverId == driverId,
                includes: new System.Linq.Expressions.Expression<Func<Order, object>>[]
                {
                    o => o.Restaurant,
                    o => o.User
                }
            );

            if (order == null)
                return NotFound("Order not found or not assigned to you.");

            return Ok(new DriverDeliveryDto(
                order.Id,
                order.Restaurant.Name,
                order.User.Name,
                order.User.Phone,
                order.TotalPrice,
                order.Status.ToString(),
                order.CreatedAt
            ));
        }

        
        [HttpPut("{orderId:int}/status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateDeliveryStatusDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var driverId = GetCurrentDriverId();

           
            var order = await _orderRepo.GetOneAsync(o => o.Id == orderId && o.DriverId == driverId);

            if (order == null)
                return NotFound("Order not found.");

           
            OrderStatus newStatus = request.Status switch
            {
                DeliveryStatus.PickedUp => OrderStatus.OnTheWay,
                DeliveryStatus.Delivered => OrderStatus.Delivered,
                _ => OrderStatus.Pending
            };

           
            if (order.Status == newStatus)
                return BadRequest("Order is already in this status.");

           
            order.Status = newStatus;

           
            if (newStatus == OrderStatus.Delivered)
            {
                order.DeliveredAt = DateTime.UtcNow;
               
            }

            
            _orderRepo.Update(order);

            
            await _orderRepo.CommitAsync();

            return Ok(new { Message = "Status updated", NewStatus = newStatus.ToString() });
        }
    }
}