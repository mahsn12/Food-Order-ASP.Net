using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FoodDelivery.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer("Data Source=.;initial catalog=FoodDeliveryDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}