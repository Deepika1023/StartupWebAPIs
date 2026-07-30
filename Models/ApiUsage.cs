namespace StartupWebAPIs.Models
{
    public class ApiUsage
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string Endpoint { get; set; } = string.Empty;

        public DateTime RequestedAt { get; set; }
            = DateTime.UtcNow;

        public int StatusCode { get; set; }

        public Customer? Customer { get; set; }
    }
}