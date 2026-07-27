namespace StartupWebAPIs.DTOs
{
    public class ProductV2Dto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Category { get; set; } = "General";

        public DateTime RetrievedOn { get; set; } = DateTime.UtcNow;
    }
}
