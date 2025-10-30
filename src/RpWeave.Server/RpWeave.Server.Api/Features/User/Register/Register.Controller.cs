using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.User.Register;

[ApiController]
public class RegisterController(RegisterHandler handler) : ControllerBase
{
    [HttpPost("api/user/register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        var result = await handler.HandleAsync(registerRequest);

        return result.ToActionResult();
    }
}