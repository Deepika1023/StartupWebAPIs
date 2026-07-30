namespace StartupWebAPIs.DTOs.Customers
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string SubscriptionPlan { get; set; } = string.Empty;
    }
}