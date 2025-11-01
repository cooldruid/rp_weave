using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Settings;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Api.Providers;

[SingletonService]
public class TokenProvider(AuthenticationSettings authenticationSettings)
{
    public string GenerateAccessToken(AppUser user, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authenticationSettings.TokenSecret));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>
        {
            new(UserClaimConstants.Id, user.Id.ToString()),
            new(UserClaimConstants.Role, role.ToUpper()),
            new(JwtRegisteredClaimNames.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var expirationDate = DateTime.UtcNow.AddHours(1);

        var jwtSecurityToken = new JwtSecurityToken(
                      authenticationSettings.Issuer,
                      authenticationSettings.Audience,
                      claimsForToken,
                      DateTime.UtcNow,
                      expirationDate,
                      signingCredentials);

        var token = new JwtSecurityTokenHandler()
           .WriteToken(jwtSecurityToken);

        return token;
    }

    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}