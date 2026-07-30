namespace StartupWebAPIs.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public int SubscriptionPlanId { get; set; }

        public SubscriptionPlan? SubscriptionPlan { get; set; }
    }
}