using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace StartupWebAPIs.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
    }
}
