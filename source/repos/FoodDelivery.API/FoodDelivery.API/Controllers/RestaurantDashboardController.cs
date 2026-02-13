using System.Security.Claims;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.Enums;
using FoodDelivery.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.API.Controllers;

[ApiController]
[Route("api/restaurant-dashboard")]
[Authorize(Roles = "Restaurant")]
public class RestaurantDashboardController(AppDbContext appDbContext) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<ActionResult<RestaurantOverviewResponse>> GetOverview()
    {
        var restaurant = await GetCurrentRestaurantAsync();
        if (restaurant is null) return NotFound("Restaurant profile was not found for this account.");

        var pendingOrders = await appDbContext.Orders.CountAsync(o => o.RestaurantId == restaurant.Id && o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled);
        var productCount = await appDbContext.Products.CountAsync(p => p.RestaurantId == restaurant.Id);

        return Ok(new RestaurantOverviewResponse(restaurant.Id, restaurant.Name, restaurant.Email, restaurant.Phone, restaurant.IsOpen, productCount, pendingOrders));
    }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<RestaurantProductResponse>>> GetProducts()
    {
        var restaurant = await GetCurrentRestaurantAsync();
        if (restaurant is null) return NotFound();

        var products = await appDbContext.Products
            .AsNoTracking()
            .Where(p => p.RestaurantId == restaurant.Id)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Ok(products.Select(p => new RestaurantProductResponse(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.IsAvailable)));
    }

    [HttpPost("products")]
    public async Task<ActionResult<RestaurantProductResponse>> CreateProduct(CreateRestaurantProductRequest request)
    {
        var restaurant = await GetCurrentRestaurantAsync();
        if (restaurant is null) return NotFound();

        var product = new Product
        {
            RestaurantId = restaurant.Id,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Price = request.Price,
            ImageUrl = request.ImageUrl?.Trim() ?? string.Empty,
            IsAvailable = request.IsAvailable
        };

        appDbContext.Products.Add(product);
        await appDbContext.SaveChangesAsync();

        return Ok(new RestaurantProductResponse(product.Id, product.Name, product.Description, product.Price, product.ImageUrl, product.IsAvailable));
    }

    [HttpPut("products/{productId:int}")]
    public async Task<IActionResult> UpdateProduct(int productId, UpdateRestaurantProductRequest request)
    {
        var restaurant = await GetCurrentRestaurantAsync();
        if (restaurant is null) return NotFound();

        var product = await appDbContext.Products.FirstOrDefaultAsync(p => p.Id == productId && p.RestaurantId == restaurant.Id);
        if (product is null) return NotFound();

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim() ?? string.Empty;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl?.Trim() ?? string.Empty;
        product.IsAvailable = request.IsAvailable;

        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<RestaurantOrderResponse>>> GetOrders()
    {
        var restaurant = await GetCurrentRestaurantAsync();
        if (restaurant is null) return NotFound();

        var orders = await appDbContext.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .Where(o => o.RestaurantId == restaurant.Id)
            .OrderByDescending(o => o.CreatedAt)
            .Take(100)
            .ToListAsync();

        return Ok(orders.Select(o => new RestaurantOrderResponse(
            o.Id,
            o.Status.ToString(),
            o.TotalPrice,
            o.CreatedAt,
            o.User?.Name ?? "Customer",
            o.OrderItems.Select(i => new RestaurantOrderItemResponse(i.ProductId, i.Product?.Name ?? "Item", i.Quantity, i.Price)).ToList())));
    }

    [HttpPut("orders/{orderId:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, UpdateOrderStatusRequest request)
    {
        var restaurant = await GetCurrentRestaurantAsync();
        if (restaurant is null) return NotFound();

        var order = await appDbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.RestaurantId == restaurant.Id);
        if (order is null) return NotFound();

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var parsedStatus))
            return BadRequest("Invalid status value.");

        order.Status = parsedStatus;
        if (parsedStatus == OrderStatus.Delivered)
        {
            order.DeliveredAt = DateTime.UtcNow;
        }

        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    private async Task<Restaurant?> GetCurrentRestaurantAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        return await appDbContext.Restaurants
            .FirstOrDefaultAsync(r => (!string.IsNullOrWhiteSpace(userId) && r.IdentityUserId == userId) || (!string.IsNullOrWhiteSpace(email) && r.Email == email));
    }
}

public record RestaurantOverviewResponse(int RestaurantId, string Name, string Email, string Phone, bool IsOpen, int ProductsCount, int ActiveOrdersCount);
public record RestaurantProductResponse(int Id, string Name, string Description, decimal Price, string ImageUrl, bool IsAvailable);
public record CreateRestaurantProductRequest(string Name, string? Description, decimal Price, string? ImageUrl, bool IsAvailable = true);
public record UpdateRestaurantProductRequest(string Name, string? Description, decimal Price, string? ImageUrl, bool IsAvailable);
public record RestaurantOrderItemResponse(int ProductId, string ProductName, int Quantity, decimal Price);
public record RestaurantOrderResponse(int OrderId, string Status, decimal TotalPrice, DateTime CreatedAt, string CustomerName, IReadOnlyCollection<RestaurantOrderItemResponse> Items);
public record UpdateOrderStatusRequest(string Status);
