using System.Security.Claims;
using FoodDelivery.Infrastructure.Data;
using FoodDelivery.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserExperienceController(
    AppDbContext appDbContext,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("restaurants")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RestaurantMenuResponse>>> GetRestaurants()
    {
        var restaurants = await appDbContext.Restaurants
            .AsNoTracking()
            .Include(r => r.Products)
            .OrderBy(r => r.Name)
            .ToListAsync();

        var response = restaurants.Select(r => new RestaurantMenuResponse(
            r.Id,
            r.Name,
            r.Description,
            r.Phone,
            r.RatingAvg,
            r.IsOpen,
            r.Products
                .Where(p => p.IsAvailable)
                .OrderBy(p => p.Name)
                .Select(p => new ProductMenuResponse(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.IsAvailable))
                .ToList()));

        return Ok(response);
    }

    [HttpGet("account")]
    public async Task<ActionResult<AccountResponse>> GetAccount()
    {
        var identityUser = await GetCurrentIdentityUserAsync();
        if (identityUser is null)
        {
            return Unauthorized();
        }

        return Ok(new AccountResponse(identityUser.FullName, identityUser.Email ?? string.Empty, identityUser.PhoneNumber));
    }

    [HttpPut("account")]
    public async Task<ActionResult<AccountResponse>> UpdateAccount(UpdateAccountRequest request)
    {
        var identityUser = await GetCurrentIdentityUserAsync();
        if (identityUser is null)
        {
            return Unauthorized();
        }

        identityUser.FullName = request.FullName.Trim();
        identityUser.PhoneNumber = request.PhoneNumber?.Trim();

        var result = await userManager.UpdateAsync(identityUser);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        return Ok(new AccountResponse(identityUser.FullName, identityUser.Email ?? string.Empty, identityUser.PhoneNumber));
    }

    [HttpPost("orders")]
    public async Task<ActionResult<PlaceOrderResponse>> PlaceOrder(PlaceOrderRequest request)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest("Order must contain at least one item.");
        }

        var domainUser = await EnsureDomainUserAsync();
        if (domainUser is null)
        {
            return Unauthorized();
        }

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await appDbContext.Products
            .Where(p => productIds.Contains(p.Id) && p.IsAvailable)
            .ToListAsync();

        if (products.Count != productIds.Distinct().Count())
        {
            return BadRequest("Some products are not available.");
        }

        var restaurantId = products.First().RestaurantId;
        if (products.Any(p => p.RestaurantId != restaurantId))
        {
            return BadRequest("All items in an order must be from the same restaurant.");
        }

        var restaurant = await appDbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId);
        if (restaurant is null)
        {
            return BadRequest("Restaurant not found.");
        }

        var itemLookup = request.Items.ToDictionary(i => i.ProductId, i => i.Quantity);
        var total = products.Sum(p => p.Price * itemLookup[p.Id]);

        var order = new FoodDelivery.Domain.Entities.Order
        {
            UserId = domainUser.Id,
            RestaurantId = restaurantId,
            Status = FoodDelivery.Domain.Enums.OrderStatus.Pending,
            TotalPrice = total,
            CreatedAt = DateTime.UtcNow,
            OrderItems = new List<FoodDelivery.Domain.Entities.OrderItem>()
        };

        foreach (var product in products)
        {
            order.OrderItems.Add(new FoodDelivery.Domain.Entities.OrderItem
            {
                ProductId = product.Id,
                Quantity = itemLookup[product.Id],
                Price = product.Price
            });
        }

        appDbContext.Orders.Add(order);
        await appDbContext.SaveChangesAsync();

        var tracking = new FoodDelivery.Domain.Entities.DeliveryTracking
        {
            OrderId = order.Id,
            Latitude = request.DeliveryLatitude,
            Longitude = request.DeliveryLongitude,
            UpdatedAt = DateTime.UtcNow
        };

        appDbContext.DeliveryTrackings.Add(tracking);
        await appDbContext.SaveChangesAsync();

        return Ok(new PlaceOrderResponse(order.Id, order.TotalPrice, order.Status.ToString(), order.CreatedAt));
    }

    [HttpGet("orders/active")]
    public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetActiveOrders()
    {
        var domainUser = await EnsureDomainUserAsync();
        if (domainUser is null)
        {
            return Unauthorized();
        }

        var activeStatuses = new[] { FoodDelivery.Domain.Enums.OrderStatus.Pending, FoodDelivery.Domain.Enums.OrderStatus.Preparing, FoodDelivery.Domain.Enums.OrderStatus.OnTheWay };

        var orders = await appDbContext.Orders
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Where(o => o.UserId == domainUser.Id && activeStatuses.Contains(o.Status))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(orders.Select(ToSummary));
    }

    [HttpGet("orders/history")]
    public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetOrderHistory()
    {
        var domainUser = await EnsureDomainUserAsync();
        if (domainUser is null)
        {
            return Unauthorized();
        }

        var orders = await appDbContext.Orders
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Where(o => o.UserId == domainUser.Id)
            .OrderByDescending(o => o.CreatedAt)
            .Take(50)
            .ToListAsync();

        return Ok(orders.Select(ToSummary));
    }

    [HttpGet("orders/{orderId:int}/tracking")]
    public async Task<ActionResult<OrderTrackingResponse>> GetTracking(int orderId)
    {
        var domainUser = await EnsureDomainUserAsync();
        if (domainUser is null)
        {
            return Unauthorized();
        }

        var order = await appDbContext.Orders
            .AsNoTracking()
            .Include(o => o.DeliveryTrackings)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == domainUser.Id);

        if (order is null)
        {
            return NotFound();
        }

        var lastPoint = order.DeliveryTrackings.OrderByDescending(t => t.UpdatedAt).FirstOrDefault();

        return Ok(new OrderTrackingResponse(order.Id, order.Status.ToString(), lastPoint?.Latitude, lastPoint?.Longitude, lastPoint?.UpdatedAt));
    }

    private async Task<ApplicationUser?> GetCurrentIdentityUserAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return string.IsNullOrWhiteSpace(userId) ? null : await userManager.FindByIdAsync(userId);
    }

    private async Task<FoodDelivery.Domain.Entities.User?> EnsureDomainUserAsync()
    {
        var identityUser = await GetCurrentIdentityUserAsync();
        if (identityUser is null || string.IsNullOrWhiteSpace(identityUser.Email))
        {
            return null;
        }

        var existing = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == identityUser.Email);
        if (existing is not null)
        {
            return existing;
        }

        var domainUser = new FoodDelivery.Domain.Entities.User
        {
            Name = identityUser.FullName,
            Email = identityUser.Email,
            Phone = identityUser.PhoneNumber ?? string.Empty,
            PasswordHash = "IDENTITY_MANAGED",
            Role = FoodDelivery.Domain.Enums.UserRole.Customer,
            CreatedAt = DateTime.UtcNow
        };

        appDbContext.Users.Add(domainUser);
        await appDbContext.SaveChangesAsync();
        return domainUser;
    }

    private static OrderSummaryResponse ToSummary(FoodDelivery.Domain.Entities.Order o)
        => new(o.Id, o.RestaurantId, o.Restaurant?.Name ?? "Unknown", o.TotalPrice, o.Status.ToString(), o.CreatedAt, o.DeliveredAt);
}

public record ProductMenuResponse(int Id, string Name, string Description, decimal Price, string ImageUrl, bool IsAvailable);
public record RestaurantMenuResponse(int Id, string Name, string Description, string Phone, double RatingAvg, bool IsOpen, IReadOnlyCollection<ProductMenuResponse> Products);
public record UpdateAccountRequest(string FullName, string? PhoneNumber);
public record AccountResponse(string FullName, string Email, string? PhoneNumber);
public record PlaceOrderItemRequest(int ProductId, int Quantity);
public record PlaceOrderRequest(IReadOnlyCollection<PlaceOrderItemRequest> Items, double DeliveryLatitude, double DeliveryLongitude);
public record PlaceOrderResponse(int OrderId, decimal TotalPrice, string Status, DateTime CreatedAt);
public record OrderSummaryResponse(int OrderId, int RestaurantId, string RestaurantName, decimal TotalPrice, string Status, DateTime CreatedAt, DateTime? DeliveredAt);
public record OrderTrackingResponse(int OrderId, string Status, double? Latitude, double? Longitude, DateTime? UpdatedAt);
