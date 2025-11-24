using Azure;
using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Api.Providers;
using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;
using RpWeave.Server.Data.Repositories;

namespace RpWeave.Server.Api.Features.User.RefreshToken;

[ScopedService]
public class RefreshTokenHandler(
    UserManager<AppUser> userManager,
    IAppUserRefreshTokenRepository refreshTokenRepository,
    TokenProvider tokenProvider)
{
    // current implementation doesnt support multi-device login
    public async Task<ValueResult<RefreshTokenResponse>> HandleAsync(HttpContext httpContext)
    {
        var refreshToken = httpContext.Request.Cookies["refreshToken"];
        
        if (string.IsNullOrEmpty(refreshToken))
            return ValueResult<RefreshTokenResponse>.Failure(ErrorCodes.NotFound, "Invalid refresh token");

        var currentRefreshToken = await refreshTokenRepository.FindByRefreshTokenAsync(refreshToken);
        if (currentRefreshToken == null)
        {
            httpContext.Response.Cookies.Delete("refreshToken");
            return ValueResult<RefreshTokenResponse>.Failure(ErrorCodes.NotFound, "Invalid refresh token");
        }

        if (currentRefreshToken.ExpiresOn < DateTime.UtcNow)
        {
            await refreshTokenRepository.DeleteAsync(currentRefreshToken);
            httpContext.Response.Cookies.Delete("refreshToken");
            return ValueResult<RefreshTokenResponse>.Failure(ErrorCodes.NotFound, "Invalid refresh token");
        }

        var user = await userManager.FindByIdAsync(currentRefreshToken.UserId);
        if(user == null)
            return ValueResult<RefreshTokenResponse>.Failure(ErrorCodes.NotFound, "Invalid refresh token");
        
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenProvider.GenerateAccessToken(user, roles.Single());
        var newRefreshToken = TokenProvider.GenerateRefreshToken();

        var expirationDate = DateTime.UtcNow.AddDays(7);
        var newRefreshTokenEntity = new AppUserRefreshToken()
        {
            UserId = user.Id.ToString(),
            RefreshToken = newRefreshToken,
            ExpiresOn = expirationDate
        };
        await refreshTokenRepository.UpsertAsync(newRefreshTokenEntity);
        httpContext.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expirationDate
        });

        var response = new RefreshTokenResponse(accessToken);
        return ValueResult<RefreshTokenResponse>.Success(response);
    }
}