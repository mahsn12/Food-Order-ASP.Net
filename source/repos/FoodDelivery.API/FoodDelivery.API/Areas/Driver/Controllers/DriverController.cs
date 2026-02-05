using FoodDelivery.Application.DTOs.Delivery;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.Enums;
using FoodDelivery.Domain.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Areas.Driver.Controllers
{
    [Area("Driver")]
    [ApiController]
    [Route("api/driver/deliveries")]
    [Authorize(Roles = "Driver")]
    public class DeliveryController : ControllerBase
    {
        private readonly IRepository<DeliveryTracking> _deliveryRepo;
        private object newStatus;

        public DeliveryController(IRepository<DeliveryTracking> deliveryRepo)
        {
            _deliveryRepo = deliveryRepo;
        }

        private string GetDriverName()
        {
            return User?.Identity?.Name
                   ?? throw new UnauthorizedAccessException("Driver not authenticated");
        }

        // GET: api/driver/deliveries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriverDto>>> GetAssignedDeliveries()
        {
            var driverName = GetDriverName();

            var deliveries = await _deliveryRepo.GetAsync(
                d => d.DriverName == driverName
            );

            var result = deliveries.Select(d => new DriverDto(
                d.OrderId,
                d.PickupLocation,
                d.DropoffLocation,
                d.Status.ToString(),
                d.ScheduledTime
            ));

            return Ok(result);
        }

        // GET: api/driver/deliveries/{orderId}
        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<DriverDto>> GetDeliveryDetails(int orderId)
        {
            var driverName = GetDriverName();

            var delivery = await _deliveryRepo.GetOneAsync(
                d => d.OrderId == orderId && d.DriverName == driverName
            );

            if (delivery == null)
                return NotFound("Delivery not found");

            return Ok(new DriverDto(
                delivery.OrderId,
                delivery.PickupLocation,
                delivery.DropoffLocation,
                delivery.Status.ToString(),
                delivery.ScheduledTime
            ));
        }

        

        [HttpPut("{orderId:int}/status")]
        public async Task<IActionResult> UpdateDeliveryStatus(
            int orderId,
            [FromBody] UpdateDeliveryStatusDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Status))
                return BadRequest("Status is required");

            var driverName = GetDriverName();

            var deliveryEntity = await _deliveryRepo.GetOneAsync(
                d => d.OrderId == orderId && d.DriverName == driverName
            );

            if (deliveryEntity == null)
                return NotFound("Delivery not found");

            if (!Enum.TryParse<DeliveryStatus>(model.Status, true, out var newStatus))
                return BadRequest("Invalid delivery status");

            // Prevent redundant updates
            if (deliveryEntity.Status is DeliveryStatus originalStatus && originalStatus == newStatus)
                return BadRequest("Delivery already in this status");

            deliveryEntity.Status = newStatus;
            await _deliveryRepo.UpdateAsync(deliveryEntity);

            return Ok(new
            {
                deliveryEntity.OrderId,
                Status = deliveryEntity.Status.ToString(),
                UpdatedAt = DateTime.UtcNow
            });
        }
    }
}
