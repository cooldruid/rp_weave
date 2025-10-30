using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.User.RefreshToken;

[ApiController]
public class RefreshTokenController(RefreshTokenHandler handler) : ControllerBase
{
    [HttpPost("/api/user/refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        var result = await handler.HandleAsync(HttpContext);

        return result.ToActionResult();
    }
}