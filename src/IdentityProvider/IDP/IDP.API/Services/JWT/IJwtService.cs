namespace IDP.API.Services.JWT;

public interface IJwtService
{
    string GenerateToken(string issuer, string[] audience, string subject, string key, int expirationMinutes);
}