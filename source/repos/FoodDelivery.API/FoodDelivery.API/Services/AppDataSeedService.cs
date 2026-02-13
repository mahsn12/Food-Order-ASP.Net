using FoodDelivery.Domain.Entities;
using FoodDelivery.Infrastructure.Data;
using FoodDelivery.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.API.Services;

public class AppDataSeedService(AppDbContext appDbContext, UserManager<ApplicationUser> userManager)
{
    public async Task SeedAsync()
    {
        if (await appDbContext.Restaurants.AnyAsync())
        {
            return;
        }

        var seedRestaurants = new List<(string Name, string Description, string Phone, string Email, string Password, List<Product> Products)>
        {
            (
                "Urban Burger",
                "Signature smashed burgers and crispy fries.",
                "+1 555 1001",
                "urban.burger@fooddelivery.local",
                "Rest@123",
                new List<Product>
                {
                    new() { Name = "Classic Smash Burger", Description = "Double beef patty, cheddar, pickles.", Price = 8.99m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd" },
                    new() { Name = "Loaded Fries", Description = "Fries topped with cheese and jalapeños.", Price = 4.50m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1630384060421-cb20d0e0649d" }
                }
            ),
            (
                "Napoli Pizza House",
                "Wood-fired pizzas made fresh daily.",
                "+1 555 1002",
                "napoli.pizza@fooddelivery.local",
                "Rest@123",
                new List<Product>
                {
                    new() { Name = "Margherita Pizza", Description = "Tomato, mozzarella, fresh basil.", Price = 11.99m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1604382354936-07c5d9983bd3" },
                    new() { Name = "Pepperoni Pizza", Description = "Loaded with premium pepperoni.", Price = 13.49m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e" }
                }
            ),
            (
                "Green Bowl",
                "Healthy bowls, wraps and juices.",
                "+1 555 1003",
                "green.bowl@fooddelivery.local",
                "Rest@123",
                new List<Product>
                {
                    new() { Name = "Chicken Caesar Bowl", Description = "Grilled chicken, romaine, parmesan.", Price = 9.75m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c" },
                    new() { Name = "Avocado Detox Smoothie", Description = "Avocado, spinach, banana, almond milk.", Price = 6.20m, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1553530666-ba11a90f281b" }
                }
            )
        };

        var restaurants = new List<Restaurant>();

        foreach (var seedRestaurant in seedRestaurants)
        {
            var owner = await userManager.FindByEmailAsync(seedRestaurant.Email);
            if (owner is null)
            {
                owner = new ApplicationUser
                {
                    UserName = seedRestaurant.Email,
                    Email = seedRestaurant.Email,
                    FullName = seedRestaurant.Name,
                    PhoneNumber = seedRestaurant.Phone,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(owner, seedRestaurant.Password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(owner, "Restaurant");
            }

            restaurants.Add(new Restaurant
            {
                Name = seedRestaurant.Name,
                Description = seedRestaurant.Description,
                Phone = seedRestaurant.Phone,
                Email = seedRestaurant.Email,
                IdentityUserId = owner.Id,
                IsOpen = true,
                RatingAvg = 4.5,
                Products = seedRestaurant.Products
            });
        }

        await appDbContext.Restaurants.AddRangeAsync(restaurants);
        await appDbContext.SaveChangesAsync();
    }
}
