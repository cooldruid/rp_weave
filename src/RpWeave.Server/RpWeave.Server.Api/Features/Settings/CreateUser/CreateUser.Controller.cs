using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.Settings.CreateUser;

[ApiController]
public class CreateUserController(CreateUserHandler handler) : ControllerBase
{
    [HttpPost("api/settings/create-user")]
    [Authorize(Policy = UserRoleConstants.Admin)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await handler.HandleAsync(request);

        return result.ToActionResult();
    }
}