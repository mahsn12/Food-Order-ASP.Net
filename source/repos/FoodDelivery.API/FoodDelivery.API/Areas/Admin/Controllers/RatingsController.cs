using FoodDelivery.Application.DTOs.Ratings;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/[area]/[controller]")]
[ApiController]
public class RatingsController(IRepository<Rating> ratingRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetAll()
    {
        var ratings = await ratingRepo.GetAsync();
        var response = ratings.Select(r => new RatingDto(r.Id, r.UserId, r.OrderId, r.RestaurantId, r.DriverId, r.RatingValue, r.Comment, r.CreatedAt));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RatingDto>> GetById(int id)
    {
        var rating = await ratingRepo.GetOneAsync(r => r.Id == id);
        if (rating is null) return NotFound();
        return Ok(new RatingDto(rating.Id, rating.UserId, rating.OrderId, rating.RestaurantId, rating.DriverId, rating.RatingValue, rating.Comment, rating.CreatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<RatingDto>> Create([FromBody] CreateRatingDto request)
    {
        var rating = new Rating
        {
            UserId = request.UserId,
            OrderId = request.OrderId,
            RestaurantId = request.RestaurantId,
            DriverId = request.DriverId,
            RatingValue = request.RatingValue,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await ratingRepo.AddAsync(rating);
        await ratingRepo.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = rating.Id }, new RatingDto(rating.Id, rating.UserId, rating.OrderId, rating.RestaurantId, rating.DriverId, rating.RatingValue, rating.Comment, rating.CreatedAt));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRatingDto request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");

        var rating = await ratingRepo.GetOneAsync(r => r.Id == id);
        if (rating is null) return NotFound();

        rating.UserId = request.UserId;
        rating.OrderId = request.OrderId;
        rating.RestaurantId = request.RestaurantId;
        rating.DriverId = request.DriverId;
        rating.RatingValue = request.RatingValue;
        rating.Comment = request.Comment;

        ratingRepo.Update(rating);
        await ratingRepo.CommitAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var rating = await ratingRepo.GetOneAsync(r => r.Id == id);
        if (rating is null) return NotFound();

        ratingRepo.Delete(rating);
        await ratingRepo.CommitAsync();
        return NoContent();
    }
}
