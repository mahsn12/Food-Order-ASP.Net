//using FoodDelivery.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using FoodDelivery.Infrastructure.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FoodDelivery.Infrastructure.Data
//{
//    public class AppDbContext : DbContext
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//        public DbSet<User> Users { get; set; }
//        public DbSet<Restaurant> Restaurants { get; set; }
//        public DbSet<Product> Products { get; set; }
//        public DbSet<Order> Orders { get; set; }
//        public DbSet<OrderItem> OrderItems { get; set; }
//        public DbSet<Payment> Payments { get; set; }
//        public DbSet<DeliveryTracking> DeliveryTrackings { get; set; }
//        public DbSet<Rating> Ratings { get; set; }


//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {


//            base.OnConfiguring(optionsBuilder);
//            optionsBuilder.UseSqlServer("Data Source=.;initial catalog=FoodDeliveryDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
//        }

//    }
//}
using FoodDelivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<DeliveryTracking> DeliveryTrackings { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>().Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderItem>().Property(oi => oi.Price).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
     .HasOne(r => r.User)
     .WithMany(u => u.Ratings)
     .HasForeignKey(r => r.UserId)
     .OnDelete(DeleteBehavior.NoAction);


        }
    }
}