using StartupWebAPIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace StartupWebAPIs.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Customer> Customers { get; set; }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        public DbSet<ApiUsage> ApiUsages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<ApiKey>()
                .HasOne(x => x.User)
                .WithMany(x => x.ApiKeys)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Price = 65000
                },
                new Product
                {
                    Id = 2,
                    Name = "Keyboard",
                    Price = 1500
                },
                new Product
                {
                    Id = 3,
                    Name = "Mouse",
                    Price = 800
                });
        }
    }
}
