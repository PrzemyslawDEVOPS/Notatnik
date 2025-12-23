using NotatnikAPI.Models;

namespace NotatnikAPI.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}

