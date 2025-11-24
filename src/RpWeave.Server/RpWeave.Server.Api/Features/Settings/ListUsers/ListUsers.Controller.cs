using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.Settings.ListUsers;

[ApiController]
public class ListUsersController(ListUsersHandler handler) : ControllerBase
{
    [HttpGet("api/settings/list-users")]
    [Authorize(Policy = UserRoleConstants.Admin)]
    public async Task<IActionResult> ListUsers()
    {
        var result = await handler.HandleAsync();

        return result.ToActionResult();
    }
}