using FoodDelivery.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace FoodDelivery.API.Services;

public class IdentitySeedService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    ILogger<IdentitySeedService> logger)
{
    public async Task SeedAdminAsync()
    {
        var email = configuration["AdminAccount:Email"];
        var password = configuration["AdminAccount:Password"];
        var fullName = configuration["AdminAccount:FullName"] ?? "System Admin";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("AdminAccount credentials are not configured. Admin seeding skipped.");
            return;
        }

        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not create Admin role: {errors}");
            }
        }

        var adminUser = await userManager.FindByEmailAsync(email);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not create admin user: {errors}");
            }

            logger.LogInformation("Admin user created with email {Email}", email);
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            var addRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not assign Admin role: {errors}");
            }
        }
    }
}
