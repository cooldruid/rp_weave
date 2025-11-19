using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.Campaign.Get;

[ApiController]
public class CampaignGetController(CampaignGetHandler handler) : ControllerBase
{
    [HttpGet("api/campaign/{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        var result = await handler.HandleAsync(id);

        return result.ToActionResult();
    }
}