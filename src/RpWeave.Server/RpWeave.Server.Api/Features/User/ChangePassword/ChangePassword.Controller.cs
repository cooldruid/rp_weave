using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.User.ChangePassword;

[ApiController]
public class ChangePasswordController(ChangePasswordHandler handler) : ControllerBase
{
    [HttpPost("api/user/change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.Claims.First(x => x.Type == UserClaimConstants.Id).Value;
        var result = await handler.HandleAsync(userId, request);

        return result.ToActionResult();
    }
}