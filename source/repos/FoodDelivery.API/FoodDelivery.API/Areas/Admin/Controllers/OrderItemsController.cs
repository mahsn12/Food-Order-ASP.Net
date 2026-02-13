using FoodDelivery.Application.DTOs.OrderItems;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/[area]/[controller]")]
[ApiController]
    [Authorize(Roles = "Admin")]
public class OrderItemsController(IRepository<OrderItem> orderItemRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll()
    {
        var items = await orderItemRepo.GetAsync();
        var response = items.Select(i => new OrderItemDto(i.Id, i.OrderId, i.ProductId, i.Quantity, i.Price));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItemDto>> GetById(int id)
    {
        var item = await orderItemRepo.GetOneAsync(i => i.Id == id);
        if (item is null) return NotFound();
        return Ok(new OrderItemDto(item.Id, item.OrderId, item.ProductId, item.Quantity, item.Price));
    }

    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create([FromBody] CreateOrderItemDto request)
    {
        var item = new OrderItem
        {
            OrderId = request.OrderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Price = request.Price
        };

        await orderItemRepo.AddAsync(item);
        await orderItemRepo.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, new OrderItemDto(item.Id, item.OrderId, item.ProductId, item.Quantity, item.Price));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderItemDto request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");

        var item = await orderItemRepo.GetOneAsync(i => i.Id == id);
        if (item is null) return NotFound();

        item.OrderId = request.OrderId;
        item.ProductId = request.ProductId;
        item.Quantity = request.Quantity;
        item.Price = request.Price;

        orderItemRepo.Update(item);
        await orderItemRepo.CommitAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await orderItemRepo.GetOneAsync(i => i.Id == id);
        if (item is null) return NotFound();

        orderItemRepo.Delete(item);
        await orderItemRepo.CommitAsync();
        return NoContent();
    }
}
