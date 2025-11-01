using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.User.Login;

[ApiController]
public class LoginController(LoginHandler handler) : ControllerBase
{
    [HttpPost("api/user/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await handler.HandleAsync(loginRequest, HttpContext);

        return result.ToActionResult();
    }
}