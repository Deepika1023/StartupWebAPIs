namespace StartupWebAPIs.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DailyLimit { get; set; }

        public int MonthlyLimit { get; set; }

        public decimal Price { get; set; }

        public ICollection<Customer> Customers { get; set; }
            = new List<Customer>();
    }
}