using FoodDelivery.Domain.Entities;
using FoodDelivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.API.Services;

public class AppDataSeedService(AppDbContext appDbContext)
{
    public async Task SeedAsync()
    {
        if (await appDbContext.Restaurants.AnyAsync())
        {
            return;
        }

        var restaurants = new List<Restaurant>
        {
            new()
            {
                Name = "Urban Burger",
                Description = "Signature smashed burgers and crispy fries.",
                Phone = "+1 555 1001",
                IsOpen = true,
                RatingAvg = 4.6,
                Products = new List<Product>
                {
                    new() { Name = "Classic Smash Burger", Description = "Double beef patty, cheddar, pickles.", Price = 8.99m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd" },
                    new() { Name = "Loaded Fries", Description = "Fries topped with cheese and jalapeños.", Price = 4.50m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1630384060421-cb20d0e0649d" }
                }
            },
            new()
            {
                Name = "Napoli Pizza House",
                Description = "Wood-fired pizzas made fresh daily.",
                Phone = "+1 555 1002",
                IsOpen = true,
                RatingAvg = 4.8,
                Products = new List<Product>
                {
                    new() { Name = "Margherita Pizza", Description = "Tomato, mozzarella, fresh basil.", Price = 11.99m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1604382354936-07c5d9983bd3" },
                    new() { Name = "Pepperoni Pizza", Description = "Loaded with premium pepperoni.", Price = 13.49m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e" }
                }
            },
            new()
            {
                Name = "Green Bowl",
                Description = "Healthy bowls, wraps and juices.",
                Phone = "+1 555 1003",
                IsOpen = true,
                RatingAvg = 4.5,
                Products = new List<Product>
                {
                    new() { Name = "Chicken Caesar Bowl", Description = "Grilled chicken, romaine, parmesan.", Price = 9.75m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c" },
                    new() { Name = "Avocado Detox Smoothie", Description = "Avocado, spinach, banana, almond milk.", Price = 6.20m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1553530666-ba11a90f281b" }
                }
            }
        };

        await appDbContext.Restaurants.AddRangeAsync(restaurants);
        await appDbContext.SaveChangesAsync();
    }
}
