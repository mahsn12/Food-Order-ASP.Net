using FoodDelivery.Application.DTOs.Orders;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/[area]/[controller]")]
[ApiController]
    [Authorize(Roles = "Admin")]
public class OrdersController(IRepository<Order> orderRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await orderRepo.GetAsync();
        var response = orders.Select(o => new OrderDto(o.Id, o.UserId, o.RestaurantId, o.DriverId, o.TotalPrice, o.Status, o.CreatedAt, o.DeliveredAt));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await orderRepo.GetOneAsync(o => o.Id == id);
        if (order is null) return NotFound();
        return Ok(new OrderDto(order.Id, order.UserId, order.RestaurantId, order.DriverId, order.TotalPrice, order.Status, order.CreatedAt, order.DeliveredAt));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto request)
    {
        var order = new Order
        {
            UserId = request.UserId,
            RestaurantId = request.RestaurantId,
            DriverId = request.DriverId,
            TotalPrice = request.TotalPrice,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        await orderRepo.AddAsync(order);
        await orderRepo.CommitAsync();

        var response = new OrderDto(order.Id, order.UserId, order.RestaurantId, order.DriverId, order.TotalPrice, order.Status, order.CreatedAt, order.DeliveredAt);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderDto request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");

        var order = await orderRepo.GetOneAsync(o => o.Id == id);
        if (order is null) return NotFound();

        order.UserId = request.UserId;
        order.RestaurantId = request.RestaurantId;
        order.DriverId = request.DriverId;
        order.TotalPrice = request.TotalPrice;
        order.Status = request.Status;
        order.DeliveredAt = request.DeliveredAt;

        orderRepo.Update(order);
        await orderRepo.CommitAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await orderRepo.GetOneAsync(o => o.Id == id);
        if (order is null) return NotFound();

        orderRepo.Delete(order);
        await orderRepo.CommitAsync();
        return NoContent();
    }
}
