namespace StartupWebAPIs.Models
{
    public class ApiKey
    {
        public int Id { get; set; }

        public string Key { get; set; } = Guid.NewGuid().ToString("N");

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }

        public User? User { get; set; }
    }
}
