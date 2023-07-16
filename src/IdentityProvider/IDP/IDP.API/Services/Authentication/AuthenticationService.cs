using IDP.API.Models;
using IDP.API.Services.JWT;

namespace IDP.API.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IJwtService jwtService, IConfiguration configuration)
    {
        _jwtService = jwtService;
        _configuration = configuration;

    }

    public string Login(UserCredentials credentials)
    {
        // Here you would typically validate the user's credentials against your user database
        if (credentials.Username != "admin" || credentials.Password != "admin")
        {
            throw new UnauthorizedAccessException();
        }

        string token = _jwtService.GenerateToken(
            issuer: _configuration["Jwt:Issuer"]!,
            audience: new string[]{
                "https://localhost:7199" // Get this from the configuration
            },
            subject: Guid.NewGuid().ToString(),
            key: _configuration["Jwt:Key"]!,
            expirationMinutes: 10);

        return token;
    }
}
