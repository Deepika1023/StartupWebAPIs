using Microsoft.EntityFrameworkCore;
using StartupWebAPIs.Models;

namespace StartupWebAPIs.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.SubscriptionPlans.AnyAsync())
                return;

            var plans = new List<SubscriptionPlan>
            {
                new SubscriptionPlan
                {
                    Name = "Free",
                    DailyLimit = 100,
                    MonthlyLimit = 3000,
                    Price = 0
                },
                new SubscriptionPlan
                {
                    Name = "Basic",
                    DailyLimit = 5000,
                    MonthlyLimit = 150000,
                    Price = 499
                },
                new SubscriptionPlan
                {
                    Name = "Pro",
                    DailyLimit = 50000,
                    MonthlyLimit = 1500000,
                    Price = 1999
                },
                new SubscriptionPlan
                {
                    Name = "Enterprise",
                    DailyLimit = int.MaxValue,
                    MonthlyLimit = int.MaxValue,
                    Price = 9999
                }
            };

            await context.SubscriptionPlans.AddRangeAsync(plans);
            await context.SaveChangesAsync();
        }
    }
}