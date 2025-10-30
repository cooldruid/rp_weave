using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Api.Providers;
using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;
using RpWeave.Server.Data.Repositories;

namespace RpWeave.Server.Api.Features.User.Login;

[ScopedService]
public class LoginHandler(
    UserManager<AppUser> userManager,
    TokenProvider tokenProvider,
    IAppUserRefreshTokenRepository appUserRefreshTokenRepository)
{
    public async Task<ValueResult<LoginResponse>> HandleAsync(LoginRequest request, HttpContext httpContext)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null)
            return ValueResult<LoginResponse>.Failure(ErrorCodes.Unauthorized, "Invalid credentials");

        var result = await userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
            return ValueResult<LoginResponse>.Failure(ErrorCodes.Unauthorized, "Invalid credentials");

        var userRoles = await userManager.GetRolesAsync(user);
        var role = userRoles.FirstOrDefault() ?? string.Empty;

        var token = tokenProvider.GenerateAccessToken(user, role.ToUpperInvariant());
        var refreshToken = TokenProvider.GenerateRefreshToken();

        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);
        var refreshTokenEntity = new AppUserRefreshToken()
        {
            UserId = user.Id.ToString(),
            RefreshToken = refreshToken,
            ExpiresOn = refreshTokenExpiration
        };
        await appUserRefreshTokenRepository.UpsertAsync(refreshTokenEntity);

        httpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        var response = new LoginResponse(token);
        return ValueResult<LoginResponse>.Success(response);
    }
}