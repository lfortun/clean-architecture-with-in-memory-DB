namespace Domain.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(string username, string role);
    DateTime GetExpiryTime();
}
