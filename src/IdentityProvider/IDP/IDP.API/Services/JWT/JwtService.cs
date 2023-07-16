using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace IDP.API.Services.JWT;

public class JwtService : IJwtService
{
    public string GenerateToken(string issuer, string[] audience, string subject, string key, int expirationMinutes)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] keyBytes = Encoding.ASCII.GetBytes(key);

        Dictionary<string, object> claims = new()
        {
            { JwtRegisteredClaimNames.Iss, issuer },
            { JwtRegisteredClaimNames.Aud, audience },
            { JwtRegisteredClaimNames.Sub, subject },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() }
        };

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Claims = claims,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }
}
