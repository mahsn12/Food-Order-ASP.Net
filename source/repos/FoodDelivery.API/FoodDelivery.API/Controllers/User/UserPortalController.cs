using System.Security.Claims;
using FoodDelivery.Application.DTOs.UserPortal;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.Enums;
using FoodDelivery.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.API.Controllers.User;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserPortalController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var domainUser = await ResolveOrCreateDomainUserAsync(cancellationToken);
        return Ok(new UserProfileDto(domainUser.Id, domainUser.Name, domainUser.Email, domainUser.Phone));
    }

    [HttpGet("restaurants")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UserRestaurantListDto>>> GetRestaurants(CancellationToken cancellationToken)
    {
        var restaurants = await dbContext.Restaurants
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new UserRestaurantListDto(r.Id, r.Name, r.Description, r.Phone, r.RatingAvg, r.IsOpen))
            .ToListAsync(cancellationToken);

        return Ok(restaurants);
    }

    [HttpGet("restaurants/{restaurantId:int}/menu")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UserProductDto>>> GetRestaurantMenu(int restaurantId, CancellationToken cancellationToken)
    {
        var restaurantExists = await dbContext.Restaurants.AnyAsync(r => r.Id == restaurantId, cancellationToken);
        if (!restaurantExists)
        {
            return NotFound($"Restaurant with ID {restaurantId} was not found.");
        }

        var products = await dbContext.Products
            .AsNoTracking()
            .Where(p => p.RestaurantId == restaurantId)
            .OrderBy(p => p.Name)
            .Select(p => new UserProductDto(p.Id, p.RestaurantId, p.Name, p.Description, p.Price, p.ImageUrl, p.IsAvailable))
            .ToListAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<UserOrderSummaryDto>>> GetOrderHistory(CancellationToken cancellationToken)
    {
        var domainUser = await ResolveOrCreateDomainUserAsync(cancellationToken);

        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == domainUser.Id)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new UserOrderSummaryDto(o.Id, o.RestaurantId, o.Restaurant.Name, o.TotalPrice, o.Status, o.CreatedAt, o.DeliveredAt))
            .ToListAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("orders/{orderId:int}")]
    public async Task<ActionResult<UserOrderDetailsDto>> GetOrderDetails(int orderId, CancellationToken cancellationToken)
    {
        var domainUser = await ResolveOrCreateDomainUserAsync(cancellationToken);

        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .Include(o => o.DeliveryTrackings)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == domainUser.Id, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        var response = new UserOrderDetailsDto(
            order.Id,
            order.UserId,
            order.RestaurantId,
            order.Restaurant.Name,
            order.TotalPrice,
            order.Status,
            order.CreatedAt,
            order.DeliveredAt,
            order.OrderItems.Select(i => new UserOrderItemDto(i.Id, i.ProductId, i.Product.Name, i.Quantity, i.Price, i.Price * i.Quantity)).ToList(),
            order.Payment is null
                ? null
                : new UserPaymentDto(order.Payment.Method, order.Payment.Status, order.Payment.TransactionRef, order.Payment.PaidAt),
            order.DeliveryTrackings
                .OrderBy(t => t.UpdatedAt)
                .Select(t => new UserDeliveryTrackingDto(t.Latitude, t.Longitude, t.UpdatedAt))
                .ToList());

        return Ok(response);
    }

    [HttpPost("orders")]
    public async Task<ActionResult<UserOrderDetailsDto>> PlaceOrder([FromBody] PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            return BadRequest("At least one item is required to place an order.");
        }

        var domainUser = await ResolveOrCreateDomainUserAsync(cancellationToken);

        var restaurant = await dbContext.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            return NotFound($"Restaurant with ID {request.RestaurantId} was not found.");
        }

        if (!restaurant.IsOpen)
        {
            return BadRequest("Restaurant is currently closed.");
        }

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await dbContext.Products
            .Where(p => p.RestaurantId == request.RestaurantId && productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
        {
            return BadRequest("One or more products are invalid for this restaurant.");
        }

        if (products.Any(p => !p.IsAvailable))
        {
            return BadRequest("One or more products are currently unavailable.");
        }

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return BadRequest("Item quantities must be greater than 0.");
            }
        }

        var itemPriceLookup = products.ToDictionary(p => p.Id, p => p.Price);
        var totalPrice = request.Items.Sum(i => itemPriceLookup[i.ProductId] * i.Quantity);

        var order = new Order
        {
            UserId = domainUser.Id,
            RestaurantId = request.RestaurantId,
            Status = OrderStatus.Pending,
            TotalPrice = totalPrice,
            CreatedAt = DateTime.UtcNow,
            OrderItems = request.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = itemPriceLookup[i.ProductId]
            }).ToList()
        };

        var payment = new Payment
        {
            Order = order,
            Method = request.PaymentMethod,
            Status = request.PaymentMethod == PaymentMethod.Cash ? PaymentStatus.Pending : PaymentStatus.Paid,
            TransactionRef = $"TXN-{Guid.NewGuid():N}"[..16],
            PaidAt = request.PaymentMethod == PaymentMethod.Cash ? null : DateTime.UtcNow
        };

        dbContext.Orders.Add(order);
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetOrderDetails), new { orderId = order.Id }, await BuildOrderDetailsDto(order.Id, domainUser.Id, cancellationToken));
    }

    private async Task<UserOrderDetailsDto> BuildOrderDetailsDto(int orderId, int userId, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .Include(o => o.DeliveryTrackings)
            .FirstAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);

        return new UserOrderDetailsDto(
            order.Id,
            order.UserId,
            order.RestaurantId,
            order.Restaurant.Name,
            order.TotalPrice,
            order.Status,
            order.CreatedAt,
            order.DeliveredAt,
            order.OrderItems.Select(i => new UserOrderItemDto(i.Id, i.ProductId, i.Product.Name, i.Quantity, i.Price, i.Price * i.Quantity)).ToList(),
            order.Payment is null
                ? null
                : new UserPaymentDto(order.Payment.Method, order.Payment.Status, order.Payment.TransactionRef, order.Payment.PaidAt),
            order.DeliveryTrackings.OrderBy(t => t.UpdatedAt).Select(t => new UserDeliveryTrackingDto(t.Latitude, t.Longitude, t.UpdatedAt)).ToList());
    }

    private async Task<User> ResolveOrCreateDomainUserAsync(CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Email claim was not found in token.");
        }

        var fullName = User.FindFirstValue("fullName") ?? email;
        var phone = User.FindFirstValue(ClaimTypes.MobilePhone) ?? string.Empty;

        var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (existingUser is not null)
        {
            if (string.IsNullOrWhiteSpace(existingUser.Name))
            {
                existingUser.Name = fullName;
                dbContext.Users.Update(existingUser);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return existingUser;
        }

        var newUser = new User
        {
            Name = fullName,
            Email = email,
            Phone = phone,
            PasswordHash = string.Empty,
            Role = UserRole.Customer,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync(cancellationToken);
        return newUser;
    }
}
