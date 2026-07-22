using StartupWebAPIs.Models;

namespace StartupWebAPIs.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
