using FoodDelivery.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Infrastructure.Data;

public class IdentityAuthDbContext : IdentityDbContext<ApplicationUser>
{
    public IdentityAuthDbContext(DbContextOptions<IdentityAuthDbContext> options) : base(options)
    {
    }
}
