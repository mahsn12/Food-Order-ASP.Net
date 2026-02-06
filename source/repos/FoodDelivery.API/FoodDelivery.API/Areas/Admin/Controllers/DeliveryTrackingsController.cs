using FoodDelivery.Application.DTOs.DeliveryTrackings;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/[area]/[controller]")]
[ApiController]
public class DeliveryTrackingsController(IRepository<DeliveryTracking> trackingRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryTrackingDto>>> GetAll()
    {
        var rows = await trackingRepo.GetAsync();
        var response = rows.Select(t => new DeliveryTrackingDto(t.Id, t.OrderId, t.Latitude, t.Longitude, t.UpdatedAt));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryTrackingDto>> GetById(int id)
    {
        var row = await trackingRepo.GetOneAsync(t => t.Id == id);
        if (row is null) return NotFound();
        return Ok(new DeliveryTrackingDto(row.Id, row.OrderId, row.Latitude, row.Longitude, row.UpdatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryTrackingDto>> Create([FromBody] CreateDeliveryTrackingDto request)
    {
        var row = new DeliveryTracking
        {
            OrderId = request.OrderId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            UpdatedAt = request.UpdatedAt ?? DateTime.UtcNow
        };

        await trackingRepo.AddAsync(row);
        await trackingRepo.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = row.Id }, new DeliveryTrackingDto(row.Id, row.OrderId, row.Latitude, row.Longitude, row.UpdatedAt));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeliveryTrackingDto request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");

        var row = await trackingRepo.GetOneAsync(t => t.Id == id);
        if (row is null) return NotFound();

        row.OrderId = request.OrderId;
        row.Latitude = request.Latitude;
        row.Longitude = request.Longitude;
        row.UpdatedAt = request.UpdatedAt ?? DateTime.UtcNow;

        trackingRepo.Update(row);
        await trackingRepo.CommitAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var row = await trackingRepo.GetOneAsync(t => t.Id == id);
        if (row is null) return NotFound();

        trackingRepo.Delete(row);
        await trackingRepo.CommitAsync();
        return NoContent();
    }
}
